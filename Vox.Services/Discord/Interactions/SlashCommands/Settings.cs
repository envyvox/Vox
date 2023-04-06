using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.CreateChannels;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Client.Extensions;
using Vox.Services.Discord.Embed;
using Vox.Services.Discord.Emotes;
using static Discord.Emote;

namespace Vox.Services.Discord.Interactions.SlashCommands;

[RequireUserPermission(GuildPermission.Administrator)]
[Group("settings", "Bot settings")]
public class Settings : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICreateChannelRepository _createChannelRepository;

    /// <inheritdoc />
    public Settings(ICreateChannelRepository createChannelRepository)
    {
        _createChannelRepository = createChannelRepository;
    }

    [SlashCommand("create-channels", "Manage created channels")]
    public async Task CreateChannelsTask()
    {
        await DeferAsync(true);

        var guildCreateChannels = await _createChannelRepository.List((long) Context.Guild.Id);

        var emotes = EmoteRepository.Emotes;
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.SettingsCreateChannels.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(Response.SettingsCreateChannelDesc.Parse(Context.Guild.PreferredLocale,
                Context.User.Mention));

        if (guildCreateChannels.Any())
        {
            var counter = 1;
            foreach (var createChannel in guildCreateChannels)
            {
                embed.AddField(
                    StringExtensions.EmptyChar,
                    Response.SettingsCreateChannelFieldDesc.Parse(Context.Guild.PreferredLocale,
                        counter, createChannel.ChannelId.ToMention(MentionType.Channel),
                        createChannel.CategoryId.ToMention(MentionType.Channel)));

                counter++;
            }
        }

        var components = new ComponentBuilder()
            .WithButton(
                Response.SettingsCreateChannelButtonCreate.Parse(Context.Guild.PreferredLocale),
                "new-create-channel",
                emote: Parse(emotes.GetEmote("VoiceChannel")),
                disabled: guildCreateChannels.Count >= 3)
            .WithButton(
                Response.SettingsCreateChannelButtonDelete.Parse(Context.Guild.PreferredLocale),
                "delete-create-channel",
                ButtonStyle.Danger,
                Parse(emotes.GetEmote("VoiceChannel")),
                disabled: guildCreateChannels.Any() is false);

        await FollowupAsync(embed: embed.Build(), components: components.Build());
    }
}
