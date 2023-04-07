using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.UserChannel.Commands;

public record SaveUserChannel(Models.UserChannel UserChannel) : IRequest;

public class SaveUserChannelHandler : IRequestHandler<SaveUserChannel>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SaveUserChannelHandler> _logger;

    public SaveUserChannelHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<SaveUserChannelHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(SaveUserChannel request, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.UserChannels
            .SingleOrDefaultAsync(x => x.UserId == request.UserChannel.UserId);

        if (entity is null)
        {
            var created = await db.CreateEntity(new Data.Entities.UserChannel
            {
                Id = Guid.NewGuid(),
                UserId = request.UserChannel.UserId,
                ChannelName = request.UserChannel.ChannelName,
                ChannelLimit = request.UserChannel.ChannelLimit,
                OverwritesData = JsonSerializer.Serialize(request.UserChannel.Overwrites as object)
            });

            _logger.LogInformation("Created user channel entity {@Entity}", created);
            return;
        }

        entity.ChannelName = request.UserChannel.ChannelName;
        entity.ChannelLimit = request.UserChannel.ChannelLimit;
        entity.OverwritesData = JsonSerializer.Serialize(request.UserChannel.Overwrites as object);

        await db.UpdateEntity(entity);
        _logger.LogInformation("Updated user channel entity {@Entity}", entity);
    }
}
