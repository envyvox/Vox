using System;
using AutoMapper;
using Vox.Services.Guild.Models;

namespace Vox.Services.GuildCreateChannel.Models;

public record GuildCreateChannelDto(
    GuildDto Guild,
    long CategoryId,
    long ChannelId,
    DateTimeOffset CreatedAt);

public class GuildCreateChannelProfile : Profile
{
    public GuildCreateChannelProfile() => CreateMap<Data.Entities.GuildCreateChannel, GuildCreateChannelDto>();
}