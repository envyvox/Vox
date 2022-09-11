using System;
using AutoMapper;
using Vox.Data.Entities;

namespace Vox.Services.Poll.Models;

public record PollAnswerDto(
    Guid Id,
    PollDto Poll,
    string Answer);

public class PollAnswerProfile : Profile
{
    public PollAnswerProfile() => CreateMap<PollAnswer, PollAnswerDto>();
}