using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Poll.Commands;

public record CreatePollCommand(int MaxAnswers, IEnumerable<string> Answers) : IRequest<Guid>;

public class CreatePollHandler : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly ILogger<CreatePollHandler> _logger;
    private readonly IMediator _mediator;
    private readonly AppDbContext _db;

    public CreatePollHandler(
        DbContextOptions options,
        ILogger<CreatePollHandler> logger,
        IMediator mediator)
    {
        _db = new AppDbContext(options);
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreatePollCommand request, CancellationToken ct)
    {
        var created = await _db.CreateEntity(new Data.Entities.Poll
        {
            Id = Guid.NewGuid(),
            MaxAnswers = request.MaxAnswers,
            CreatedAt = DateTimeOffset.UtcNow
        });

        await _mediator.Send(new CreatePollAnswersCommand(created.Id, request.Answers));

        _logger.LogInformation(
            "Created poll entity {@Entity}",
            created);

        return created.Id;
    }
}