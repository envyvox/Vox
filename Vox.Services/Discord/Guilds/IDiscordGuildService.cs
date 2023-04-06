using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Vox.Services.Discord.Guilds;

public interface IDiscordGuildService
{
    Task<SocketGuild> GetSocketGuild(ulong guildId);
    Task<SocketTextChannel> GetSocketTextChannel(ulong guildId, ulong channelId);
    Task<IUserMessage> GetUserMessage(ulong guildId, ulong channelId, ulong messageId);
}
