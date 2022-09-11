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

public record GetAllUserPollAnswersQuery(Guid PollId) : IRequest<List<UserPollAnswerDto>>;

public class GetAllUserPollAnswersHandler : IRequestHandler<GetAllUserPollAnswersQuery, List<UserPollAnswerDto>>
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _db;

    public GetAllUserPollAnswersHandler(
        DbContextOptions options,
        IMapper mapper)
    {
        _mapper = mapper;
        _db = new AppDbContext(options);
    }

    public async Task<List<UserPollAnswerDto>> Handle(GetAllUserPollAnswersQuery request, CancellationToken ct)
    {
        var entities = await _db.UserPollAnswers
            .Include(x => x.Answer)
            .Where(x => x.PollId == request.PollId)
            .ToListAsync();

        return _mapper.Map<List<UserPollAnswerDto>>(entities);
    }
}