using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vox.Data;
using Vox.Data.Extensions;
using Vox.Services.Guild.Models;

namespace Vox.Services.Guild.Commands;

public record CreateGuildEntityCommand(long Id) : IRequest<GuildDto>;

public class CreateGuildEntityHandler : IRequestHandler<CreateGuildEntityCommand, GuildDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateGuildEntityHandler> _logger;

    public CreateGuildEntityHandler(
        DbContextOptions options,
        IMapper mapper,
        ILogger<CreateGuildEntityHandler> logger)
    {
        _db = new AppDbContext(options);
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GuildDto> Handle(CreateGuildEntityCommand request, CancellationToken ct)
    {
        var exist = await _db.Guilds
            .AnyAsync(x => x.Id == request.Id);

        if (exist)
        {
            throw new Exception(
                $"guild with id {request.Id} already exist in database");
        }

        var created = await _db.CreateEntity(new Data.Entities.Guild
        {
            Id = request.Id,
            CreateRoomLimit = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        });

        _logger.LogInformation(
            "Created guild entity {@Entity}",
            created);

        return _mapper.Map<GuildDto>(created);
    }
}