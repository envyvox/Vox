using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.GuildCreateChannel.Commands;

public record CreateGuildCreateChannelCommand(long GuildId, long CategoryId, long ChannelId) : IRequest;

public class CreateGuildCreateChannelHandler : IRequestHandler<CreateGuildCreateChannelCommand>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CreateGuildCreateChannelHandler> _logger;

    public CreateGuildCreateChannelHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<CreateGuildCreateChannelHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Handle(CreateGuildCreateChannelCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var exist = await db.GuildCreateChannels
            .AnyAsync(x =>
                x.GuildId == request.GuildId &&
                x.CategoryId == request.CategoryId);

        if (exist)
        {
            throw new Exception(
                $"guild {request.GuildId} already have create channel in category {request.CategoryId}");
        }

        var created = await db.CreateEntity(new Data.Entities.GuildCreateChannel
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
    }
}
