using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Hangfire;
using MediatR;
using Vox.Data.Enums;
using Vox.Services.Discord.Emote.Extensions;
using Vox.Services.Discord.Extensions;
using Vox.Services.Extensions;
using Vox.Services.Hangfire.CompletePoll;
using Vox.Services.Poll.Commands;
using static Discord.Emote;
using static Vox.Services.Extensions.ExceptionExtensions;
using ComponentType = Vox.Data.Enums.ComponentType;

namespace Vox.Services.Discord.Interactions.SlashCommands;

[RequireUserPermission(GuildPermission.Administrator)]
public class CreatePoll : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public CreatePoll(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SlashCommand("create-poll", "Creates a poll to select the specified answers (max 25)")]
    public async Task Execute(
        [Summary("type", "Poll type")] ComponentType componentType,
        [Summary("duration", "Poll duration in minutes")] [MinValue(1)]
        int durationInMinutes,
        [Summary("question", "Question for which you would like to collect answers")]
        string question,
        [Summary("answers", "Answer options separated by commas ,")]
        string inputAnswers,
        [Summary("max-answers", "Number of options that can be selected at the same time (default 1)")]
        [MinValue(1), MaxValue(25)]
        int maxAnswers = 1)
    {
        await DeferAsync(true);

        var answers = inputAnswers
            .Split(',')
            .Distinct()
            .ToArray();

        if (answers.Length is < 2 or > 25)
        {
            throw new ExpectedException(Response.WrongAnswersCount.Parse(Context.Guild.PreferredLocale));
        }

        if (answers.Any(answer => answer.Length > StringExtensions.LabelMaxChars))
        {
            throw new ExpectedException(Response.MaxCharsLimitation.Parse(Context.Guild.PreferredLocale,
                StringExtensions.LabelMaxChars));
        }

        var pollId = Guid.NewGuid();
        var emotes = DiscordRepository.Emotes;
        var components = new ComponentBuilder();
        var pollEmbed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.Poll.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .AddField(
                Response.PollQuestion.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("QA")),
                question)
            .AddField(
                Response.PollDuration.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Timer")),
                $"{durationInMinutes} {Response.Minutes.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, durationInMinutes)}")
            .AddField(
                Response.PollMaxAnswers.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("QA")),
                $"{maxAnswers} {Response.Answers.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, maxAnswers)}")
            .WithFooter(Response.PollFooter.Parse(Context.Guild.PreferredLocale));

        switch (componentType)
        {
            case ComponentType.Buttons:
            {
                foreach (var answer in answers)
                {
                    components.WithButton(
                        answer,
                        $"poll-answer-button:{pollId},{answer}",
                        emote: Parse(emotes.GetEmote("QA")));
                }

                break;
            }
            case ComponentType.Dropdown:
            {
                var selectMenu = new SelectMenuBuilder()
                    .WithCustomId($"poll-answer-dropdown:{pollId}")
                    .WithPlaceholder(
                        Response.SelectAnswersFromDropdown.Parse(Context.Guild.PreferredLocale, maxAnswers))
                    .WithMaxValues(maxAnswers);

                foreach (var answer in answers)
                {
                    selectMenu.AddOption(answer, answer, emote: Parse(emotes.GetEmote("QA")));
                }

                components.WithSelectMenu(selectMenu);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(componentType), componentType, null);
        }

        var avatarUrl = Context.User.GetAvatarUrl();
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.CreatePoll.Parse(Context.Guild.PreferredLocale),
                avatarUrl)
            .WithDescription(Response.SuccessfullyCreatedPoll.Parse(Context.Guild.PreferredLocale,
                Context.User.Mention));

        var message = await Context.Channel.SendMessageAsync(embed: pollEmbed.Build(), components: components.Build());

        await _mediator.Send(new CreatePollCommand(
            pollId, (long) Context.Guild.Id, (long) Context.Channel.Id, (long) message.Id, maxAnswers));

        BackgroundJob.Schedule<ICompletePollJob>(
            x => x.Execute(pollId, Context.Guild.Id, Context.Channel.Id, message.Id, question, avatarUrl),
            TimeSpan.FromMinutes(durationInMinutes));

        await FollowupAsync(embed: embed.Build());
    }
}