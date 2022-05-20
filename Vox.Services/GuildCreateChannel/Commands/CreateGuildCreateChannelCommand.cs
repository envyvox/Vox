using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.GuildCreateChannel.Commands;

public record CreateGuildCreateChannelCommand(long GuildId, long CategoryId, long ChannelId) : IRequest;

public class CreateGuildCreateChannelHandler : IRequestHandler<CreateGuildCreateChannelCommand>
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreateGuildCreateChannelHandler> _logger;

    public CreateGuildCreateChannelHandler(
        DbContextOptions options,
        ILogger<CreateGuildCreateChannelHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateGuildCreateChannelCommand request, CancellationToken ct)
    {
        var exist = await _db.GuildCreateChannels
            .AnyAsync(x =>
                x.GuildId == request.GuildId &&
                x.CategoryId == request.CategoryId);

        if (exist)
        {
            throw new Exception(
                $"guild {request.GuildId} already have create channel in category {request.CategoryId}");
        }

        var created = await _db.CreateEntity(new Data.Entities.GuildCreateChannel
        {
            Id = Guid.NewGuid(),
            GuildId = request.GuildId,
            CategoryId = request.CategoryId,
            ChannelId = request.ChannelId,
            CreatedAt = DateTimeOffset.UtcNow
        });

        _logger.LogInformation(
            "Created guild create channel entity {@Entity}",
            created);

        return Unit.Value;
    }
}