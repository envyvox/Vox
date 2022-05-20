using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using MediatR;
using Vox.Data.Enums;
using Vox.Services.Discord.Extensions;
using Vox.Services.Extensions;
using Vox.Services.Poll.Commands;
using Vox.Services.Poll.Queries;
using static Vox.Services.Extensions.ExceptionExtensions;

namespace Vox.Services.Discord.Interactions.Components.Poll;

public class PollAnswerDropdown : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public PollAnswerDropdown(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ComponentInteraction("poll-answer-dropdown:*")]
    public async Task Execute(string pollIdString, string[] answers)
    {
        await DeferAsync(true);

        var poll = await _mediator.Send(new GetPollQuery(Guid.Parse(pollIdString)));
        var userAnswers = await _mediator.Send(new GetUserPollAnswersQuery((long) Context.User.Id, poll.Id));

        var newAnswerCount = answers.Count(answer => userAnswers.Select(x => x.Answer).Contains(answer) is false);

        if (userAnswers.Count + newAnswerCount > poll.MaxAnswers)
        {
            throw new ExpectedException(Response.PollAnswersLimitation.Parse(Context.Guild.PreferredLocale,
                poll.MaxAnswers));
        }

        foreach (var answer in answers)
        {
            if (userAnswers.Select(x => x.Answer).Contains(answer)) continue;

            await _mediator.Send(new CreatePollAnswerCommand((long) Context.User.Id, poll.Id, answer));
        }

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.Poll.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(Response.PollAnswerDesc.Parse(Context.Guild.PreferredLocale,
                Context.User.Mention, answers
                    .Aggregate(string.Empty, (s, v) => s + $"{v}, ")
                    .RemoveFromEnd(2)));

        await FollowupAsync(embed: embed.Build(), ephemeral: true);
    }
}