using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using MediatR;
using Microsoft.Extensions.Logging;
using Vox.Data.Enums;
using Vox.Services.Discord.Emote.Extensions;
using Vox.Services.Discord.Extensions;
using Vox.Services.Discord.Guild.Queries;
using Vox.Services.Extensions;
using Vox.Services.Poll.Commands;
using Vox.Services.Poll.Queries;

namespace Vox.Services.Hangfire.CompletePoll;

public class CompletePollJob : ICompletePollJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompletePollJob> _logger;

    public CompletePollJob(IMediator mediator, ILogger<CompletePollJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Execute(Guid pollId, ulong guildId, ulong channelId, ulong messageId, string question,
        string avatarUrl)
    {
        var message = await _mediator.Send(new GetUserMessageQuery(guildId, channelId, messageId));
        if (message is null)
        {
            _logger.LogError(
                "Cannot find message {MessageId} in guild {GuildId} channel {ChannelId} while executing complete poll job for poll {PollId}",
                messageId, guildId, channelId, pollId);
            return;
        }

        var poll = await _mediator.Send(new GetPollQuery(pollId));
        var pollAnswers = await _mediator.Send(new GetAllUserPollAnswersQuery(poll.Id));
        var guild = await _mediator.Send(new GetSocketGuildQuery(guildId));

        // count each answer
        var answers = pollAnswers
            .GroupBy(x => x.Answer)
            .Select(x => new {x.Key, Count = x.Count()})
            .OrderByDescending(x => x.Count);

        var emotes = DiscordRepository.Emotes;
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.Poll.Parse(guild.PreferredLocale),
                avatarUrl)
            .AddField(
                Response.PollQuestion.Parse(guild.PreferredLocale,
                    emotes.GetEmote("QA")),
                question)
            .AddField(
                Response.PollResults.Parse(guild.PreferredLocale,
                    emotes.GetEmote("QA")),
                answers.Aggregate(string.Empty, (s, v) =>
                    s +
                    $"`{v.Count} {Response.Answers.Parse(guild.PreferredLocale).Localize(guild.PreferredLocale, v.Count)}`: {v.Key.Answer}\n"));

        await message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = new ComponentBuilder().Build();
        });

        await _mediator.Send(new DeletePollCommand(poll.Id));

        _logger.LogInformation(
            "Complete poll job for poll {PollId} executed successfully and poll deleted",
            pollId);
    }
}