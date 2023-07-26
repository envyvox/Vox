using System;
using AutoMapper;

namespace Vox.Services.Guild.Models;

public record GuildDto(
    long Id,
    bool Removed,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public class GuildProfile : Profile
{
    public GuildProfile() => CreateMap<Data.Entities.Guild, GuildDto>();
}