using System;
using AutoMapper;
using Vox.Services.Guild.Models;

namespace Vox.Services.Poll.Models;

public record PollDto(
    Guid Id,
    GuildDto Guild,
    long ChannelId,
    long MessageId,
    int MaxAnswers,
    DateTimeOffset CreatedAt);

public class PollProfile : Profile
{
    public PollProfile() => CreateMap<Data.Entities.Poll, PollDto>();
}