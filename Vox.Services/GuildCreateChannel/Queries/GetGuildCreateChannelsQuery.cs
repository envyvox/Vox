using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vox.Data;
using Vox.Services.GuildCreateChannel.Models;

namespace Vox.Services.GuildCreateChannel.Queries;

public record GetGuildCreateChannelsQuery(long GuildId) : IRequest<List<GuildCreateChannelDto>>;

public class GetGuildCreateChannelsHandler : IRequestHandler<GetGuildCreateChannelsQuery, List<GuildCreateChannelDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public GetGuildCreateChannelsHandler(
        DbContextOptions options,
        IMapper mapper)
    {
        _db = new AppDbContext(options);
        _mapper = mapper;
    }

    public async Task<List<GuildCreateChannelDto>> Handle(GetGuildCreateChannelsQuery request, CancellationToken ct)
    {
        var entities = await _db.GuildCreateChannels
            .Include(x => x.Guild)
            .Where(x => x.GuildId == request.GuildId)
            .ToListAsync();

        return _mapper.Map<List<GuildCreateChannelDto>>(entities);
    }
}