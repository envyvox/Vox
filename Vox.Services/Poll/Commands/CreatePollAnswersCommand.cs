using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Entities;
using Vox.Data.Extensions;
using static Vox.Services.Extensions.ExceptionExtensions;

namespace Vox.Services.Poll.Commands;

public record CreatePollAnswersCommand(Guid PollId, IEnumerable<string> Answers) : IRequest;

public class CreatePollAnswersHandler : IRequestHandler<CreatePollAnswersCommand>
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreatePollAnswersHandler> _logger;

    public CreatePollAnswersHandler(
        DbContextOptions options,
        ILogger<CreatePollAnswersHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(CreatePollAnswersCommand request, CancellationToken cancellationToken)
    {
        foreach (var answer in request.Answers)
        {
            var exist = _db.PollAnswers.Any(x =>
                x.PollId == request.PollId &&
                x.Answer == answer);

            if (exist)
            {
                throw new ExpectedException(
                    $"poll {request.PollId} answer {answer} already exist");
            }

            var created = await _db.CreateEntity(new PollAnswer
            {
                Id = Guid.NewGuid(),
                PollId = request.PollId,
                Answer = answer
            });

            _logger.LogInformation(
                "Created poll answer entity {@Entity}",
                created);
        }

        return Unit.Value;
    }
}