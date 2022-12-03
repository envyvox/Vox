using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vox.Data;
using Vox.Services.Guild.Commands;
using Vox.Services.Guild.Models;

namespace Vox.Services.Guild.Queries;

public record GetGuildEntityQuery(long Id) : IRequest<GuildDto>;

public class GetGuildEntityHandler : IRequestHandler<GetGuildEntityQuery, GuildDto>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GetGuildEntityHandler(
        IServiceScopeFactory scopeFactory,
        IMediator mediator,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<GuildDto> Handle(GetGuildEntityQuery request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.Guilds
            .SingleOrDefaultAsync(x => x.Id == request.Id);

        return entity is null
            ? await _mediator.Send(new CreateGuildEntityCommand(request.Id))
            : _mapper.Map<GuildDto>(entity);
    }
}