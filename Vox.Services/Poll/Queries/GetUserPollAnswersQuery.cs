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

public record GetUserPollAnswersQuery(long UserId, Guid PollId) : IRequest<List<UserPollAnswerDto>>;

public class GetUserPollAnswersHandler : IRequestHandler<GetUserPollAnswersQuery, List<UserPollAnswerDto>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;


    public GetUserPollAnswersHandler(
        IServiceScopeFactory scopeFactory,
        IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public async Task<List<UserPollAnswerDto>> Handle(GetUserPollAnswersQuery request, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entities = await db.UserPollAnswers
            .Include(x => x.Answer)
            .Where(x =>
                x.UserId == request.UserId &&
                x.PollId == request.PollId)
            .ToListAsync();

        return _mapper.Map<List<UserPollAnswerDto>>(entities);
    }
}
