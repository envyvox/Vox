using System;
using AutoMapper;

namespace Vox.Services.Guild.Models;

public record GuildDto(
    long Id,
    int CreateRoomLimit,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public class GuildProfile : Profile
{
    public GuildProfile() => CreateMap<Data.Entities.Guild, GuildDto>();
}