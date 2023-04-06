using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Vox.Framework.Autofac;
using Vox.Framework.Database;

namespace Vox.Services.UserChannels;

[InjectableService]
public class UserChannelRepository : IUserChannelRepository
{
    private readonly IConnectionManager _con;
    private readonly ILogger<UserChannelRepository> _logger;
    private readonly VoxOverwritePermissions _defaultPermissions = new(285212688, 0);

    public UserChannelRepository(
        IConnectionManager con,
        ILogger<UserChannelRepository> logger)
    {
        _con = con;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserChannel> Get(SocketUser socketUser)
    {
        var dto = await _con
            .GetConnection()
            .QuerySingleOrDefaultAsync<(
                Guid Id,
                long UserId,
                string ChannelName,
                int ChannelLimit,
                string OverwritesData)?>(@"
                select id as Id,
                       user_id as UserId,
                       channel_name as ChannelName,
                       channel_limit as ChannelLimit,
                       permissions as OverwritesData
                from user_channel
                where user_id = @UserId",
                new { UserId = (long) socketUser.Id });

        if (dto.HasValue)
        {
            return new UserChannel(
                dto.Value.Id,
                dto.Value.UserId,
                dto.Value.ChannelName,
                dto.Value.ChannelLimit,
                JsonSerializer.Deserialize<IReadOnlyCollection<VoxOverwrite>>(dto.Value.OverwritesData) ??
                new List<VoxOverwrite>());
        }

        var userChannel = new UserChannel(
            Guid.NewGuid(),
            (long) socketUser.Id,
            socketUser.Username,
            5,
            new List<VoxOverwrite> { new(socketUser.Id, PermissionTarget.User, _defaultPermissions) });

        await Save(userChannel);
        return userChannel;
    }

    /// <inheritdoc />
    public async Task Save(UserChannel userChannel)
    {
        var dto = new
        {
            Id = userChannel.Id,
            UserId = userChannel.UserId,
            ChannelName = userChannel.ChannelName,
            ChannelLimit = userChannel.ChannelLimit,
            Permissions = JsonSerializer.Serialize(userChannel.Overwrites as object)
        };

        await _con
            .GetConnection()
            .ExecuteAsync(@"
                insert into user_channel(id, user_id, channel_name, channel_limit, permissions)
                values (@Id, @UserId, @ChannelName, @ChannelLimit, @Permissions)
                on conflict (user_id) do update
                    set channel_name  = @ChannelName,
                        channel_limit = @ChannelLimit,
                        permissions   = @Permissions;", dto);

        _logger.LogInformation("User channel saved {@Entity}", dto);
    }
}
