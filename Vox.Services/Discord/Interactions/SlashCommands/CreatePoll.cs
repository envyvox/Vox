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
using Vox.Services.Poll.Queries;
using static Discord.Emote;
using static Vox.Services.Extensions.ExceptionExtensions;
using ComponentType = Vox.Data.Enums.ComponentType;

namespace Vox.Services.Discord.Interactions.SlashCommands;

[RequireUserPermission(GuildPermission.Administrator)]
public class CreatePoll : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;
    private const int MaxAnswers = 25;
    private const int AnswerMaxLenght = 60;

    public CreatePoll(IMediator mediator)
    {
        _mediator = mediator;
    }

    [SlashCommand("create-poll", "Creates a poll to select the specified answers (max 20)")]
    public async Task Execute(
        [Summary("type", "Poll type")] ComponentType componentType,
        [Summary("duration", "Poll duration in minutes")] [MinValue(1)]
        int durationInMinutes,
        [Summary("question", "Question for which you would like to collect answers")]
        string question,
        [Summary("answers", "Answers separated by ; symbol (ex: 1;2;3)")]
        string providedAnswers,
        [Summary("mention-everyone", "Adds @everyone to the message")]
        bool mentionEveryone,
        [Summary("create-thread", "Creates a thread from the message")]
        bool createThread,
        [Summary("max-answers", "Number of options that can be selected at the same time (default 1)")]
        [MinValue(1)]
        [MaxValue(25)]
        int maxAnswers = 1)
    {
        await DeferAsync(true);

        var answers = providedAnswers
            .Split(";")
            .Where(x => string.IsNullOrWhiteSpace(x) == false)
            .Distinct()
            .ToArray();

        if (answers.Length > MaxAnswers)
        {
            throw new ExpectedException(Response.CreatePollAnswersLimitation.Parse(Context.Guild.PreferredLocale));
        }

        if (answers.Any(x => x.Length > AnswerMaxLenght))
        {
            throw new ExpectedException(Response.CreatePollAnswerTooLong.Parse(Context.Guild.PreferredLocale));
        }

        var pollId = await _mediator.Send(new CreatePollCommand(maxAnswers, answers));
        var pollAnswers = await _mediator.Send(new GetPollAnswersQuery(pollId));

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
                foreach (var answer in pollAnswers)
                {
                    components.WithButton(
                        answer.Answer,
                        $"poll-answer-button:{pollId},{answer.Id}",
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

                foreach (var answer in pollAnswers)
                {
                    selectMenu.AddOption(answer.Answer, $"{answer.Id}", emote: Parse(emotes.GetEmote("QA")));
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

        var message = await Context.Channel.SendMessageAsync(
            mentionEveryone ? "@everyone" : null,
            embed: pollEmbed.Build(),
            components: components.Build());

        if (createThread)
        {
            await (Context.Channel as ITextChannel)!.CreateThreadAsync(
                question,
                autoArchiveDuration: ThreadArchiveDuration.OneWeek,
                message: message)!;
        }

        BackgroundJob.Schedule<ICompletePollJob>(
            x => x.Execute(pollId, Context.Guild.Id, Context.Channel.Id, message.Id, question, avatarUrl),
            TimeSpan.FromMinutes(durationInMinutes));

        await FollowupAsync(embed: embed.Build());
    }
}