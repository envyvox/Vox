using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.Discord.Emote.Extensions;
using Vox.Services.Discord.Extensions;
using Vox.Services.Extensions;
using static Discord.Emote;
using StringExtensions = Vox.Services.Extensions.StringExtensions;

namespace Vox.Services.Discord.Interactions.Components.View;

public class ViewGuildRoles : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("view-guild-roles:*")]
    public async Task Execute(string pageString)
    {
        await DeferAsync(true);

        var page = int.Parse(pageString);
        var maxPages = (int) Math.Ceiling(Context.Guild.Roles.Count / 10.0);
        var roles = Context.Guild.Roles
            .OrderBy(x => x.Position)
            .Skip(page > 1 ? (page - 1) * 10 : 0)
            .Take(10);

        var emotes = DiscordRepository.Emotes;
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.ViewGuildRoles.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(roles.Aggregate(string.Empty, (s, v) =>
                s +
                Response.GuildRoleInfo.Parse(Context.Guild.PreferredLocale,
                    v.Position, v.Mention, emotes.GetEmote("Brush"), v.Color.ToString(),
                    emotes.GetEmote("User"), v.Members.Count(x => x.IsBot is false),
                    Response.User.Parse(Context.Guild.PreferredLocale)
                        .Localize(Context.Guild.PreferredLocale, v.Members.Count(x => x.IsBot is false)),
                    emotes.GetEmote("Bot"), v.Members.Count(x => x.IsBot),
                    Response.Bot.Parse(Context.Guild.PreferredLocale)
                        .Localize(Context.Guild.PreferredLocale, v.Members.Count(x => x.IsBot)))))
            .WithFooter(
                Response.FooterPage.Parse(Context.Guild.PreferredLocale,
                    page, maxPages),
                StringExtensions.PageImageUrl);

        var components = new ComponentBuilder()
            .WithButton(
                Response.Back.Parse(Context.Guild.PreferredLocale),
                $"view-guild-roles:{page - 1}",
                emote: Parse(emotes.GetEmote("ArrowLeft")),
                disabled: page <= 1)
            .WithButton(
                Response.Forward.Parse(Context.Guild.PreferredLocale),
                $"view-guild-roles:{page + 1}",
                emote: Parse(emotes.GetEmote("ArrowRight")),
                disabled: page >= maxPages);

        await ModifyOriginalResponseAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = components.Build();
        });
    }
}