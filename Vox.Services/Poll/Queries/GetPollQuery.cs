using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vox.Data;
using Vox.Services.Poll.Models;

namespace Vox.Services.Poll.Queries;

public record GetPollQuery(Guid Id) : IRequest<PollDto>;

public class GetPollHandler : IRequestHandler<GetPollQuery, PollDto>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;

    public GetPollHandler(
        DbContextOptions options,
        IMapper mapper)
    {
        _db = new AppDbContext(options);
        _mapper = mapper;
    }

    public async Task<PollDto> Handle(GetPollQuery request, CancellationToken ct)
    {
        var entity = await _db.Polls
            .SingleOrDefaultAsync(x => x.Id == request.Id);

        if (entity is null)
        {
            throw new Exception($"poll {request.Id} not found");
        }

        return _mapper.Map<PollDto>(entity);
    }
}