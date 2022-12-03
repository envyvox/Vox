using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Poll.Commands;

public record CreatePollCommand(int MaxAnswers, IEnumerable<string> Answers) : IRequest<Guid>;

public class CreatePollHandler : IRequestHandler<CreatePollCommand, Guid>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CreatePollHandler> _logger;
    private readonly IMediator _mediator;

    public CreatePollHandler(
        IServiceScopeFactory scopeFactory,
        ILogger<CreatePollHandler> logger,
        IMediator mediator)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreatePollCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var created = await db.CreateEntity(new Data.Entities.Poll
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