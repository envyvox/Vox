using System.Threading.Tasks;
using Discord.WebSocket;

namespace Vox.Services.UserChannels;

public interface IUserChannelRepository
{
    Task<UserChannel> Get(SocketUser socketUser);
    Task Save(UserChannel userChannel);
}
