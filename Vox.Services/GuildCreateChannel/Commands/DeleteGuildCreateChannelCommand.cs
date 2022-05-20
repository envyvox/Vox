using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.GuildCreateChannel.Commands;

public record DeleteGuildCreateChannelCommand(long GuildId, long CategoryId) : IRequest;

public class DeleteGuildCreateChannelHandler : IRequestHandler<DeleteGuildCreateChannelCommand>
{
    private readonly ILogger<DeleteGuildCreateChannelHandler> _logger;
    private readonly AppDbContext _db;

    public DeleteGuildCreateChannelHandler(
        DbContextOptions options,
        ILogger<DeleteGuildCreateChannelHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteGuildCreateChannelCommand request, CancellationToken ct)
    {
        var entity = await _db.GuildCreateChannels
            .SingleOrDefaultAsync(x =>
                x.GuildId == request.GuildId &&
                x.CategoryId == request.CategoryId);

        if (entity is null)
        {
            throw new Exception(
                $"guild {request.GuildId} doesnt have created channel category {request.CategoryId} in database");
        }

        await _db.DeleteEntity(entity);

        _logger.LogInformation(
            "Deleted guild create channel entity {@Entity}",
            entity);

        return Unit.Value;
    }
}