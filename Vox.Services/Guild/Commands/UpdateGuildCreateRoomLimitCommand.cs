using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Guild.Commands;

public record UpdateGuildCreateRoomLimitCommand(long GuildId, int CreateRoomLimit) : IRequest;

public class UpdateGuildCreateRoomLimitHandler : IRequestHandler<UpdateGuildCreateRoomLimitCommand>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateGuildCreateRoomLimitHandler> _logger;


    public UpdateGuildCreateRoomLimitHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<UpdateGuildCreateRoomLimitHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Handle(UpdateGuildCreateRoomLimitCommand request, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.Guilds
            .SingleOrDefaultAsync(x => x.Id == request.GuildId);

        if (entity is null)
        {
            throw new Exception(
                $"Guild {request.GuildId} not found in database");
        }

        entity.CreateRoomLimit = request.CreateRoomLimit;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await db.UpdateEntity(entity);

        _logger.LogInformation(
            "Updated guild {GuildId} create room limit to {CreateRoomLimit}",
            request.GuildId, request.CreateRoomLimit);
    }
}
