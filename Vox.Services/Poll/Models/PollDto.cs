using System;
using AutoMapper;

namespace Vox.Services.Poll.Models;

public record PollDto(
    Guid Id,
    int MaxAnswers,
    DateTimeOffset CreatedAt);

public class PollProfile : Profile
{
    public PollProfile() => CreateMap<Data.Entities.Poll, PollDto>();
}