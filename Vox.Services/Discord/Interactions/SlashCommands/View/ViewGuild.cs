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

namespace Vox.Services.Discord.Interactions.SlashCommands.View;

public class ViewGuild : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("view-guild", "View information about a guild")]
    public async Task Execute()
    {
        await DeferAsync(true);

        // make sure all guild users are cached
        await Context.Guild.DownloadUsersAsync();

        var emotes = EmoteRepository.Emotes;
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithThumbnailUrl($"https://cdn.discordapp.com/icons/{Context.Guild.Id}/{Context.Guild.IconId}.png")
            .WithAuthor(
                Response.ViewGuild.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .AddField(Response.FieldName.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Text")),
                Context.Guild.Name,
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldOwner.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Crown")),
                Context.Guild.Owner.Mention,
                true)
            .AddField(Response.FieldMembersCount.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("User")),
                $"{emotes.GetEmote("User")} {Context.Guild.Users.Count(x => x.IsBot is false)} {Response.User.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, Context.Guild.Users.Count(x => x.IsBot is false))}, " +
                $"{emotes.GetEmote("Bot")} {Context.Guild.Users.Count(x => x.IsBot)} {Response.Bot.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, Context.Guild.Users.Count(x => x.IsBot))}",
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldRolesCount.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Role")),
                $"{Context.Guild.Roles.Count} {Response.Role.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, Context.Guild.Roles.Count)}",
                true)
            .AddField(Response.FieldTextChannelsCount.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("TextChannel")),
                $"{Context.Guild.VoiceChannels.Count} {Response.Channel.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, Context.Guild.VoiceChannels.Count)}",
                true)
            .AddEmptyField(true)
            .AddField(Response.FieldVoiceChannelsCount.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("VoiceChannel")),
                $"{Context.Guild.VoiceChannels.Count} {Response.Channel.Parse(Context.Guild.PreferredLocale).Localize(Context.Guild.PreferredLocale, Context.Guild.VoiceChannels.Count)}",
                true)
            .AddField(Response.FieldCreatedAt.Parse(Context.Guild.PreferredLocale,
                    emotes.GetEmote("Calendar")),
                Context.Guild.CreatedAt)
            .WithImageUrl($"https://cdn.discordapp.com/banners/{Context.Guild.Id}/{Context.Guild.BannerId}.png");

        var components = new ComponentBuilder()
            .WithButton(
                Response.ViewGuildRoles.Parse(Context.Guild.PreferredLocale),
                "view-guild-roles:1",
                emote: Parse(emotes.GetEmote("Folder")));

        await FollowupAsync(embed: embed.Build(), components: components.Build());
    }
}
