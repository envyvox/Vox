using System.Threading.Tasks;
using Discord.WebSocket;

namespace Vox.Services.Discord.Client;

public interface IDiscordClientService
{
    public Task Start();
    public Task<DiscordSocketClient> GetSocketClient();
}
