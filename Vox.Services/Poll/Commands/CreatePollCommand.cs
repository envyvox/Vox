using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;

namespace Vox.Services.Poll.Commands;

public record CreatePollCommand(Guid Id, long GuildId, long ChannelId, long MessageId, int MaxAnswers) : IRequest;

public class CreatePollHandler : IRequestHandler<CreatePollCommand>
{
    private readonly ILogger<CreatePollHandler> _logger;
    private readonly AppDbContext _db;

    public CreatePollHandler(
        DbContextOptions options,
        ILogger<CreatePollHandler> logger)
    {
        _db = new AppDbContext(options);
        _logger = logger;
    }

    public async Task<Unit> Handle(CreatePollCommand request, CancellationToken ct)
    {
        var exist = await _db.Polls
            .AnyAsync(x =>
                x.GuildId == request.GuildId &&
                x.ChannelId == request.ChannelId &&
                x.MessageId == request.MessageId);

        if (exist)
        {
            throw new Exception(
                $"poll entity with guild {request.GuildId}, channel {request.ChannelId} and message {request.MessageId} already exist");
        }

        var created = await _db.CreateEntity(new Data.Entities.Poll
        {
            Id = request.Id,
            GuildId = request.GuildId,
            ChannelId = request.ChannelId,
            MessageId = request.MessageId,
            MaxAnswers = request.MaxAnswers,
            CreatedAt = DateTimeOffset.UtcNow
        });

        _logger.LogInformation(
            "Created poll entity {@Entity}",
            created);

        return Unit.Value;
    }
}