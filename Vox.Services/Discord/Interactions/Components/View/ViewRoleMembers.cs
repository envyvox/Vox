using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Client.Extensions;
using Vox.Services.Discord.Embed;
using Vox.Services.Discord.Emotes;
using static Discord.Emote;

namespace Vox.Services.Discord.Interactions.Components.View;

public class ViewRoleMembers : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("view-role-members:*,*")]
    public async Task Execute(string roleIdString, string pageString)
    {
        await DeferAsync(true);

        var roleId = ulong.Parse(roleIdString);
        var page = int.Parse(pageString);

        var role = Context.Guild.GetRole(roleId);
        var maxPages = (int) Math.Ceiling(role.Members.Count() / 25.0);
        var roleMembers = role.Members
            .Skip(page > 1 ? (page - 1) * 25 : 0)
            .Take(25);

        var emotes = EmoteRepository.Emotes;
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.ViewRoleMembers.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithFooter(
                Response.FooterPage.Parse(Context.Guild.PreferredLocale,
                    page, maxPages),
                StringExtensions.PageImageUrl);

        foreach (var socketGuildUser in roleMembers)
        {
            embed.AddField(
                socketGuildUser.IsBot
                    ? Response.BotTag.Parse(Context.Guild.PreferredLocale, emotes.GetEmote("Bot"))
                    : Response.UserTag.Parse(Context.Guild.PreferredLocale, emotes.GetEmote("User")),
                socketGuildUser.Mention, true);
        }

        var components = new ComponentBuilder()
            .WithButton(
                Response.Back.Parse(Context.Guild.PreferredLocale),
                $"view-role-members:{roleIdString},{page - 1}",
                emote: Parse(emotes.GetEmote("ArrowLeft")),
                disabled: page <= 1)
            .WithButton(
                Response.Forward.Parse(Context.Guild.PreferredLocale),
                $"view-role-members:{roleIdString},{page + 1}",
                emote: Parse(emotes.GetEmote("ArrowRight")),
                disabled: page >= maxPages);

        await ModifyOriginalResponseAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = components.Build();
        });
    }
}
