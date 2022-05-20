using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using MediatR;
using Vox.Data.Enums;
using Vox.Services.Discord.Extensions;
using Vox.Services.Poll.Commands;
using Vox.Services.Poll.Queries;
using static Vox.Services.Extensions.ExceptionExtensions;

namespace Vox.Services.Discord.Interactions.Components.Poll;

public class PollAnswerButton : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public PollAnswerButton(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ComponentInteraction("poll-answer-button:*,*")]
    public async Task Execute(string pollIdString, string answer)
    {
        await DeferAsync(true);

        var poll = await _mediator.Send(new GetPollQuery(Guid.Parse(pollIdString)));
        var userAnswers = await _mediator.Send(new GetUserPollAnswersQuery((long) Context.User.Id, poll.Id));

        if (userAnswers.Select(x => x.Answer).Contains(answer))
        {
            throw new ExpectedException(Response.PollAnswersAlready.Parse(Context.Guild.PreferredLocale));
        }

        if (userAnswers.Count >= poll.MaxAnswers)
        {
            throw new ExpectedException(Response.PollAnswersLimitation.Parse(Context.Guild.PreferredLocale,
                poll.MaxAnswers));
        }

        await _mediator.Send(new CreatePollAnswerCommand((long) Context.User.Id, poll.Id, answer));

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.Poll.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(Response.PollAnswerDesc.Parse(Context.Guild.PreferredLocale,
                Context.User.Mention, answer));

        await FollowupAsync(embed: embed.Build(), ephemeral: true);
    }
}