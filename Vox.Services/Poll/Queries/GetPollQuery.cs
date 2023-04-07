using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vox.Data;
using Vox.Services.Poll.Models;

namespace Vox.Services.Poll.Queries;

public record GetPollQuery(Guid Id) : IRequest<PollDto>;

public class GetPollHandler : IRequestHandler<GetPollQuery, PollDto>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;


    public GetPollHandler(
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public async Task<PollDto> Handle(GetPollQuery request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.Polls
            .SingleOrDefaultAsync(x => x.Id == request.Id);

        if (entity is null)
        {
            throw new Exception($"poll {request.Id} not found");
        }

        return _mapper.Map<PollDto>(entity);
    }
}