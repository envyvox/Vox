using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Entities;
using Vox.Data.Extensions;

namespace Vox.Services.Poll.Commands;

public record CreatePollAnswerCommand(long UserId, Guid PollId, string Answer) : IRequest;

public class CreatePollAnswerHandler : IRequestHandler<CreatePollAnswerCommand>
{
    private readonly ILogger<CreatePollAnswerHandler> _logger;
    private readonly AppDbContext _db;

    public CreatePollAnswerHandler(
        DbContextOptions options,
        ILogger<CreatePollAnswerHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(CreatePollAnswerCommand request, CancellationToken ct)
    {
        var exist = await _db.PollAnswers
            .AnyAsync(x =>
                x.UserId == request.UserId &&
                x.PollId == request.PollId &&
                x.Answer == request.Answer);

        if (exist)
        {
            throw new Exception(
                $"poll answer entity for user {request.UserId} and poll {request.PollId} with answer {request.Answer} already exist");
        }

        var created = await _db.CreateEntity(new PollAnswer
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PollId = request.PollId,
            Answer = request.Answer,
            CreatedAt = DateTimeOffset.UtcNow
        });

        _logger.LogInformation(
            "Created poll answer entity {@Entity}",
            created);

        return Unit.Value;
    }
}