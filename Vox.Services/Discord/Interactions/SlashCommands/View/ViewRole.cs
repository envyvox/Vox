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

namespace Vox.Services.Discord.Interactions.SlashCommands.View;

public class ViewRole : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("view-role", "View information about a specified role")]
    public async Task Execute(
        [Summary("role", "Role")] IRole mentionedRole)
    {
        await DeferAsync(true);

        var role = Context.Guild.GetRole(mentionedRole.Id);

        var emotes = EmoteRepository.Emotes;
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithThumbnailUrl(role.GetIconUrl())
            .WithAuthor(
                Response.ViewRole.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .AddField(Response.FieldName.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Text")),
                role.Name,
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldMention.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Paperclip")),
                role.Mention,
                true)
            .AddField(Response.FieldId.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("ID")),
                role.Id,
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldColor.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Brush")),
                role.Color.ToString(),
                true)
            .AddField(Response.FieldMembersCount.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("User")),
                $"{emotes.GetEmote("User")} {role.Members.Count(x => x.IsBot is false)} {Response.User.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, role.Members.Count(x => x.IsBot is false))}, " +
                $"{emotes.GetEmote("Bot")} {role.Members.Count(x => x.IsBot)} {Response.Bot.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, role.Members.Count(x => x.IsBot))}")
            .AddField(Response.FieldCreatedAt.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Calendar")),
                role.CreatedAt);

        var components = new ComponentBuilder()
            .WithButton(
                Response.ViewRoleMembers.Parse(Context.Guild.PreferredLocale),
                $"view-role-members:{role.Id},1",
                emote: Parse(emotes.GetEmote("Folder")));

        await FollowupAsync(embed: embed.Build(), components: components.Build());
    }
}
