using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Vox.Framework.Autofac;
using Vox.Services.Discord.Client;

namespace Vox.Services.Discord.Guilds;

[InjectableService]
public class DiscordGuildService : IDiscordGuildService
{
    private readonly IDiscordClientService _discordClientService;

    public DiscordGuildService(IDiscordClientService discordClientService)
    {
        _discordClientService = discordClientService;
    }

    /// <inheritdoc />
    public async Task<SocketGuild> GetSocketGuild(ulong guildId)
    {
        var client = await _discordClientService.GetSocketClient();
        var guild = client.GetGuild(guildId);

        if (guild is null)
        {
            throw new Exception(
                $"guild {guildId} not found");
        }

        return guild;
    }

    /// <inheritdoc />
    public async Task<SocketTextChannel> GetSocketTextChannel(ulong guildId, ulong channelId)
    {
        var guild = await GetSocketGuild(guildId);
        var channel = guild.GetTextChannel(channelId);

        if (channel is null)
        {
            throw new Exception(
                $"channel {channelId} not found in guild {guildId}");
        }

        return channel;
    }

    /// <inheritdoc />
    public async Task<IUserMessage> GetUserMessage(ulong guildId, ulong channelId, ulong messageId)
    {
        var channel = await GetSocketTextChannel(guildId, channelId);
        return (IUserMessage) await channel.GetMessageAsync(messageId);
    }
}
