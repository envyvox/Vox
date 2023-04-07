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
    public async Task Execute(string pollIdString, string[] answersId)
    {
        await DeferAsync(true);

        var poll = await _mediator.Send(new GetPollQuery(Guid.Parse(pollIdString)));
        var pollAnswers = await _mediator.Send(new GetPollAnswersQuery(poll.Id));
        var userAnswers = await _mediator.Send(new GetUserPollAnswersQuery((long) Context.User.Id, poll.Id));
        var selectedAnswers = (
                from pollAnswer in pollAnswers
                from answerId in answersId
                where pollAnswer.Id == Guid.Parse(answerId)
                select pollAnswer)
            .ToList();

        var newAnswerCount = selectedAnswers.Count(pollAnswer => userAnswers
            .Select(userPollAnswer => userPollAnswer.Answer)
            .Contains(pollAnswer) is false);

        if (userAnswers.Count + newAnswerCount > poll.MaxAnswers)
        {
            throw new ExpectedException(Response.PollAnswersLimitation.Parse(Context.Guild.PreferredLocale,
                poll.MaxAnswers));
        }

        foreach (var answer in selectedAnswers.Where(answer => !userAnswers.Select(x => x.Answer).Contains(answer)))
        {
            await _mediator.Send(new CreateUserPollAnswerCommand((long) Context.User.Id, poll.Id, answer.Id));
        }

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.Poll.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(Response.PollAnswerDesc.Parse(Context.Guild.PreferredLocale,
                Context.User.Mention, selectedAnswers
                    .Aggregate(string.Empty, (s, v) => s + $"{v.Answer}, ")
                    .RemoveFromEnd(2)));

        await FollowupAsync(embed: embed.Build(), ephemeral: true);
    }
}