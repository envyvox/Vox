using System;
using AutoMapper;
using Vox.Data.Entities;

namespace Vox.Services.Poll.Models;

public record PollAnswerDto(
    long UserId,
    PollDto Poll,
    string Answer,
    DateTimeOffset CreatedAt);

public class PollAnswerProfile : Profile
{
    public PollAnswerProfile() => CreateMap<PollAnswer, PollAnswerDto>();
}