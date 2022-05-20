using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.Discord.Emote.Extensions;
using Vox.Services.Discord.Extensions;
using Vox.Services.Extensions;

namespace Vox.Services.Discord.Interactions.SlashCommands.View;

public class ViewUser : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("view-user", "View information about a specified user")]
    public async Task Execute(
        [Summary("user", "User")] IUser mentionedUser)
    {
        await DeferAsync(true);

        var user = Context.Guild.GetUser(mentionedUser.Id);

        var emotes = DiscordRepository.Emotes;
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithThumbnailUrl(user.GetDisplayAvatarUrl())
            .WithAuthor(
                Response.ViewUser.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .AddField(Response.FieldName.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Text")),
                user.DisplayName,
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldMention.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Paperclip")),
                user.Mention,
                true)
            .AddField(Response.FieldId.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("ID")),
                user.Id,
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldRolesCount.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Role")),
                $"{user.Roles.Count} {Response.Role.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, user.Roles.Count)}",
                true)
            .AddField(Response.FieldCreatedAt.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Calendar")),
                user.CreatedAt,
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldJoinedAt.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Calendar")),
                user.JoinedAt,
                true);

        await FollowupAsync(embed: embed.Build());
    }
}