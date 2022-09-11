using System;
using AutoMapper;
using Vox.Data.Entities;

namespace Vox.Services.Poll.Models;

public record UserPollAnswerDto(
    long UserId,
    PollDto Poll,
    PollAnswerDto Answer,
    DateTimeOffset CreatedAt);

public class UserPollAnswerProfile : Profile
{
    public UserPollAnswerProfile() => CreateMap<UserPollAnswer, UserPollAnswerDto>();
}