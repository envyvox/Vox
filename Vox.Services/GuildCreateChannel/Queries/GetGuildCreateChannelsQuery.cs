using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vox.Data;
using Vox.Services.GuildCreateChannel.Models;

namespace Vox.Services.GuildCreateChannel.Queries;

public record GetGuildCreateChannelsQuery(long GuildId) : IRequest<List<GuildCreateChannelDto>>;

public class GetGuildCreateChannelsHandler : IRequestHandler<GetGuildCreateChannelsQuery, List<GuildCreateChannelDto>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public GetGuildCreateChannelsHandler(
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public async Task<List<GuildCreateChannelDto>> Handle(GetGuildCreateChannelsQuery request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entities = await db.GuildCreateChannels
            .Include(x => x.Guild)
            .Where(x => x.GuildId == request.GuildId)
            .ToListAsync();

        return _mapper.Map<List<GuildCreateChannelDto>>(entities);
    }
}