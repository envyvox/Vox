using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vox.Services.CreateChannels;

public interface ICreateChannelRepository
{
    Task<IReadOnlyCollection<CreateChannel>> List(long guildId);
    Task Create(CreateChannel channel);
    Task Delete(long guildId, long categoryId);
}
