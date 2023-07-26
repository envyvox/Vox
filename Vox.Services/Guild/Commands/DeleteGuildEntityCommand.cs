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

public record DeleteGuildEntityCommand(long Id) : IRequest;

public class DeleteGuildEntityHandler : IRequestHandler<DeleteGuildEntityCommand>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeleteGuildEntityHandler> _logger;

    public DeleteGuildEntityHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<DeleteGuildEntityHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Handle(DeleteGuildEntityCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.Guilds
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (entity is null)
        {
            throw new Exception(
                $"Guild with id {request.Id} not exist in database");
        }

        entity.Removed = true;

        await db.UpdateEntity(entity);

        _logger.LogInformation(
            "Updated guild entity {@Entity} with removed flag",
            entity);
    }
}