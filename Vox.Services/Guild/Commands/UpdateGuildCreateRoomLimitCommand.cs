using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Guild.Commands;

public record UpdateGuildCreateRoomLimitCommand(long GuildId, int CreateRoomLimit) : IRequest;

public class UpdateGuildCreateRoomLimitHandler : IRequestHandler<UpdateGuildCreateRoomLimitCommand>
{
    private readonly ILogger<UpdateGuildCreateRoomLimitHandler> _logger;
    private readonly AppDbContext _db;

    public UpdateGuildCreateRoomLimitHandler(
        DbContextOptions options,
        ILogger<UpdateGuildCreateRoomLimitHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateGuildCreateRoomLimitCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Guilds
            .SingleOrDefaultAsync(x => x.Id == request.GuildId);

        if (entity is null)
        {
            throw new Exception(
                $"Guild {request.GuildId} not found in database");
        }

        entity.CreateRoomLimit = request.CreateRoomLimit;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.UpdateEntity(entity);

        _logger.LogInformation(
            "Updated guild {GuildId} create room limit to {CreateRoomLimit}",
            request.GuildId, request.CreateRoomLimit);

        return Unit.Value;
    }
}