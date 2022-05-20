using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Poll.Commands;

public record DeletePollCommand(Guid PollId) : IRequest;

public class DeletePollHandler : IRequestHandler<DeletePollCommand>
{
    private readonly ILogger<DeletePollHandler> _logger;
    private readonly AppDbContext _db;

    public DeletePollHandler(
        DbContextOptions options,
        ILogger<DeletePollHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(DeletePollCommand request, CancellationToken ct)
    {
        var entity = await _db.Polls
            .SingleOrDefaultAsync(x => x.Id == request.PollId);

        if (entity is null)
        {
            throw new Exception(
                $"poll {request.PollId} not found in database");
        }

        await _db.DeleteEntity(entity);

        _logger.LogInformation(
            "Deleted poll entity {@Entity}",
            entity);

        return Unit.Value;
    }
}