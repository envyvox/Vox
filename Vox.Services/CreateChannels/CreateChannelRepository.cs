using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Vox.Framework.Autofac;
using Vox.Framework.Database;

namespace Vox.Services.CreateChannels;

[InjectableService]
public class CreateChannelRepository : ICreateChannelRepository
{
    private readonly IConnectionManager _con;
    private readonly ILogger<CreateChannelRepository> _logger;

    public CreateChannelRepository(
        IConnectionManager con,
        ILogger<CreateChannelRepository> logger)
    {
        _con = con;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CreateChannel>> List(long guildId)
    {
        return (await _con
                .GetConnection()
                .QueryAsync<CreateChannel>(@"
                    select id as Id,
                           guild_id as GuildId,
                           category_id as CategoryId,
                           channel_id as ChannelId
                    from create_channel
                    where guild_id = @guildId",
                    new { guildId }))
            .ToList();
    }

    /// <inheritdoc />
    public async Task Create(CreateChannel channel)
    {
        var dto = new
        {
            Id = channel.Id,
            GuildId = channel.GuildId,
            CategoryId = channel.CategoryId,
            ChannelId = channel.ChannelId
        };

        await _con
            .GetConnection()
            .ExecuteAsync(@"
                insert into create_channel(id, guild_id, category_id, channel_id)
                values (@Id, @GuildId, @CategoryId, @ChannelId)", dto);

        _logger.LogInformation("Created create channel entity {@Entity}", dto);
    }

    /// <inheritdoc />
    public async Task Delete(long guildId, long categoryId)
    {
        await _con
            .GetConnection()
            .ExecuteAsync(
                "delete from create_channel where guild_id = @guildId and category_id = @categoryId",
                new { guildId, categoryId });

        _logger.LogInformation(
            "Deleted create channel entity with GuildId: {GuildId} and CategoryId: {CategoryId}",
            guildId, categoryId);
    }
}
