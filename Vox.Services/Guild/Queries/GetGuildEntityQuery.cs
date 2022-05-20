using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vox.Data;
using Vox.Services.Guild.Commands;
using Vox.Services.Guild.Models;

namespace Vox.Services.Guild.Queries;

public record GetGuildEntityQuery(long Id) : IRequest<GuildDto>;

public class GetGuildEntityHandler : IRequestHandler<GetGuildEntityQuery, GuildDto>
{
    private readonly AppDbContext _db;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GetGuildEntityHandler(
        DbContextOptions options,
        IMediator mediator,
        IMapper mapper)
    {
        _db = new AppDbContext(options);
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<GuildDto> Handle(GetGuildEntityQuery request, CancellationToken ct)
    {
        var entity = await _db.Guilds
            .SingleOrDefaultAsync(x => x.Id == request.Id);

        if (entity is null) return await _mediator.Send(new CreateGuildEntityCommand(request.Id));
        return _mapper.Map<GuildDto>(entity);
    }
}