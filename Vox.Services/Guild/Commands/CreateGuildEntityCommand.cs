using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;
using Vox.Services.Guild.Models;

namespace Vox.Services.Guild.Commands;

public record CreateGuildEntityCommand(long Id) : IRequest<GuildDto>;

public class CreateGuildEntityHandler : IRequestHandler<CreateGuildEntityCommand, GuildDto>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateGuildEntityHandler> _logger;

    public CreateGuildEntityHandler(
        IServiceScopeFactory scopeFactory,
        IMapper mapper,
        ILogger<CreateGuildEntityHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GuildDto> Handle(CreateGuildEntityCommand request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var exist = await db.Guilds
            .AnyAsync(x => x.Id == request.Id);

        if (exist)
        {
            throw new Exception(
                $"guild with id {request.Id} already exist in database");
        }

        var created = await db.CreateEntity(new Data.Entities.Guild
        {
            Id = request.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });

        _logger.LogInformation(
            "Created guild entity {@Entity}",
            created);

        return _mapper.Map<GuildDto>(created);
    }
}
