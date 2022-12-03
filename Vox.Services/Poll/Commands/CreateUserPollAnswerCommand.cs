using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Entities;
using Vox.Data.Extensions;

namespace Vox.Services.Poll.Commands;

public record CreateUserPollAnswerCommand(long UserId, Guid PollId, Guid AnswerId) : IRequest;

public class CreateUserPollAnswerHandler : IRequestHandler<CreateUserPollAnswerCommand>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CreateUserPollAnswerHandler> _logger;

    public CreateUserPollAnswerHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<CreateUserPollAnswerHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<Unit> Handle(CreateUserPollAnswerCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var exist = await db.UserPollAnswers
            .AnyAsync(x =>
                x.UserId == request.UserId &&
                x.PollId == request.PollId &&
                x.AnswerId == request.AnswerId);

        if (exist)
        {
            throw new Exception(
                $"poll answer entity for user {request.UserId} and poll {request.PollId} with answer {request.AnswerId} already exist");
        }

        var created = await db.CreateEntity(new UserPollAnswer
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PollId = request.PollId,
            AnswerId = request.AnswerId,
            CreatedAt = DateTimeOffset.UtcNow
        });

        _logger.LogInformation(
            "Created user poll answer entity {@Entity}",
            created);

        return Unit.Value;
    }
}