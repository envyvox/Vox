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

public record DeleteGuildCreateChannelCommand(long GuildId, long CategoryId) : IRequest;

public class DeleteGuildCreateChannelHandler : IRequestHandler<DeleteGuildCreateChannelCommand>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeleteGuildCreateChannelHandler> _logger;

    public DeleteGuildCreateChannelHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<DeleteGuildCreateChannelHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Handle(DeleteGuildCreateChannelCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.GuildCreateChannels
            .SingleOrDefaultAsync(x =>
                x.GuildId == request.GuildId &&
                x.CategoryId == request.CategoryId);

        if (entity is null)
        {
            throw new Exception(
                $"guild {request.GuildId} doesnt have created channel category {request.CategoryId} in database");
        }

        await db.DeleteEntity(entity);

        _logger.LogInformation(
            "Deleted guild create channel entity {@Entity}",
            entity);
    }
}
