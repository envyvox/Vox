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

    [SlashCommand("create-poll", "Creates a poll to select the specified answers (max 20)")]
    public async Task Execute(
        [Summary("type", "Poll type")] ComponentType componentType,
        [Summary("duration", "Poll duration in minutes")] [MinValue(1)]
        int durationInMinutes,
        [Summary("question", "Question for which you would like to collect answers")]
        string question,
        [Summary("answer1", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer1,
        [Summary("answer2", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer2,
        [Summary("answer3", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer3 = null,
        [Summary("answer4", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer4 = null,
        [Summary("answer5", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer5 = null,
        [Summary("answer6", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer6 = null,
        [Summary("answer7", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer7 = null,
        [Summary("answer8", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer8 = null,
        [Summary("answer9", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer9 = null,
        [Summary("answer10", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer10 = null,
        [Summary("answer11", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer11 = null,
        [Summary("answer12", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer12 = null,
        [Summary("answer13", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer13 = null,
        [Summary("answer14", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer14 = null,
        [Summary("answer15", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer15 = null,
        [Summary("answer16", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer16 = null,
        [Summary("answer17", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer17 = null,
        [Summary("answer18", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer18 = null,
        [Summary("answer19", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer19 = null,
        [Summary("answer20", "Answer")] [MinLength(1)] [MaxLength(80)]
        string answer20 = null,
        [Summary("max-answers", "Number of options that can be selected at the same time (default 1)")]
        [MinValue(1), MaxValue(25)]
        int maxAnswers = 1)
    {
        await DeferAsync(true);

        var answers = new[]
            {
                answer1, answer2, answer3, answer4, answer5, answer6, answer7, answer8, answer9, answer10, answer11,
                answer12, answer13, answer14, answer15, answer16, answer17, answer18, answer19, answer20
            }
            .Where(x => x is not null && x is not "")
            .Distinct()
            .ToArray();

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

        var message = await Context.Channel.SendMessageAsync(embed: pollEmbed.Build(), components: components.Build());


        BackgroundJob.Schedule<ICompletePollJob>(
            x => x.Execute(pollId, Context.Guild.Id, Context.Channel.Id, message.Id, question, avatarUrl),
            TimeSpan.FromMinutes(durationInMinutes));

        await FollowupAsync(embed: embed.Build());
    }
}