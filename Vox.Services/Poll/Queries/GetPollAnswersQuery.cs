using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vox.Data;
using Vox.Services.Poll.Models;

namespace Vox.Services.Poll.Queries;

public record GetPollAnswersQuery(Guid PollId) : IRequest<List<PollAnswerDto>>;

public class GetPollAnswersHandler : IRequestHandler<GetPollAnswersQuery, List<PollAnswerDto>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;


    public GetPollAnswersHandler(
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public async Task<List<PollAnswerDto>> Handle(GetPollAnswersQuery request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entities = await db.PollAnswers
            .AsQueryable()
            .Where(x => x.PollId == request.PollId)
            .ToListAsync();

        return _mapper.Map<List<PollAnswerDto>>(entities);
    }
}