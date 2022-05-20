using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Guild.Commands;

public record DeleteGuildEntityCommand(long Id) : IRequest;

public class DeleteGuildEntityHandler : IRequestHandler<DeleteGuildEntityCommand>
{
    private readonly AppDbContext _db;
    private readonly ILogger<DeleteGuildEntityHandler> _logger;

    public DeleteGuildEntityHandler(
        DbContextOptions options,
        ILogger<DeleteGuildEntityHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(DeleteGuildEntityCommand request, CancellationToken ct)
    {
        var entity = await _db.Guilds
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (entity is null)
        {
            throw new Exception(
                $"Guild with id {request.Id} not exist in database");
        }

        await _db.DeleteEntity(entity);

        _logger.LogInformation(
            "Deleted guild entity {@Entity}",
            entity);

        return Unit.Value;
    }
}