using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Poll.Commands;

public record DeletePollCommand(Guid PollId) : IRequest;

public class DeletePollHandler : IRequestHandler<DeletePollCommand>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeletePollHandler> _logger;


    public DeletePollHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<DeletePollHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<Unit> Handle(DeletePollCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.Polls
            .SingleOrDefaultAsync(x => x.Id == request.PollId);

        if (entity is null)
        {
            throw new Exception(
                $"poll {request.PollId} not found in database");
        }

        await db.DeleteEntity(entity);

        _logger.LogInformation(
            "Deleted poll entity {@Entity}",
            entity);

        return Unit.Value;
    }
}