using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vox.Data;
using Vox.Services.Poll.Models;

namespace Vox.Services.Poll.Queries;

public record GetUserPollAnswersQuery(long UserId, Guid PollId) : IRequest<List<PollAnswerDto>>;

public class GetUserPollAnswersHandler : IRequestHandler<GetUserPollAnswersQuery, List<PollAnswerDto>>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;

    public GetUserPollAnswersHandler(
        DbContextOptions options,
        IMapper mapper)
    {
        _db = new AppDbContext(options);
        _mapper = mapper;
    }

    public async Task<List<PollAnswerDto>> Handle(GetUserPollAnswersQuery request, CancellationToken ct)
    {
        var entities = await _db.PollAnswers
            .AsQueryable()
            .Where(x =>
                x.UserId == request.UserId &&
                x.PollId == request.PollId)
            .ToListAsync();

        return _mapper.Map<List<PollAnswerDto>>(entities);
    }
}