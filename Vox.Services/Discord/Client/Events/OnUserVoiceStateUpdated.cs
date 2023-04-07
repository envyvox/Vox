using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MediatR;
using Vox.Services.Guild.Queries;
using Vox.Services.GuildCreateChannel.Models;
using Vox.Services.GuildCreateChannel.Queries;
using Vox.Services.UserChannel.Commands;
using Vox.Services.UserChannel.Models;
using Vox.Services.UserChannel.Queries;

namespace Vox.Services.Discord.Client.Events;

public record OnUserVoiceStateUpdated(
        SocketUser SocketUser,
        SocketVoiceState OldSocketVoiceState,
        SocketVoiceState NewSocketVoiceState)
    : IRequest;

public class OnUserVoiceStateUpdatedHandler : IRequestHandler<OnUserVoiceStateUpdated>
{
    private readonly IMediator _mediator;

    public OnUserVoiceStateUpdatedHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(OnUserVoiceStateUpdated request, CancellationToken ct)
    {
        var oldChannel = request.OldSocketVoiceState.VoiceChannel;
        var newChannel = request.NewSocketVoiceState.VoiceChannel;
        var socketGuild = oldChannel is null ? newChannel.Guild : oldChannel.Guild;
        var guild = await _mediator.Send(new GetGuildEntityQuery((long) socketGuild.Id));
        var guildCreateChannels = await _mediator.Send(new GetGuildCreateChannelsQuery(guild.Id));

        if (guildCreateChannels.Count < 1) return;

        var userChannel = await _mediator.Send(new GetUserChannel(request.SocketUser));

        if (IsCreateVoiceChannel(newChannel, guildCreateChannels))
        {
            var createdChannel = await socketGuild.CreateVoiceChannelAsync(userChannel.ChannelName, x =>
            {
                x.CategoryId = (ulong) guildCreateChannels
                    .Single(createChannel => createChannel.ChannelId == (long) newChannel.Id)
                    .CategoryId;
                x.UserLimit = userChannel.ChannelLimit;
            });

            await ((SocketGuildUser) request.SocketUser).ModifyAsync(x => x.Channel = createdChannel);
            await ApplyPermissionsToChannel(createdChannel, socketGuild,
                VoxOverwrite.ToDiscordOverwrites(userChannel.Overwrites));
        }

        if (WasInCreatedChannelsCategory(oldChannel, guildCreateChannels))
        {
            userChannel.UpdateChannelName(oldChannel!.Name);
            userChannel.UpdateChannelLimit(oldChannel.UserLimit);
            userChannel.UpdateOverwrites(oldChannel.PermissionOverwrites);

            await _mediator.Send(new SaveUserChannel(userChannel));
            await oldChannel.DeleteAsync();
        }
    }

    private static async Task ApplyPermissionsToChannel(RestVoiceChannel? channel, SocketGuild? guild,
        IReadOnlyCollection<Overwrite>? overwrites)
    {
        if (channel is null)
        {
            throw new Exception("Rest voice channel is null");
        }

        if (guild is null)
        {
            throw new Exception("Socket guild is null");
        }

        if (overwrites is null)
        {
            throw new Exception("Error parsing permissions");
        }

        foreach (var overwrite in overwrites)
        {
            switch (overwrite.TargetType)
            {
                case PermissionTarget.Role:
                    var role = guild.GetRole(overwrite.TargetId);
                    await channel.AddPermissionOverwriteAsync(role, overwrite.Permissions);
                    break;
                case PermissionTarget.User:
                    var user = guild.GetUser(overwrite.TargetId);
                    await channel.AddPermissionOverwriteAsync(user, overwrite.Permissions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static bool IsCreateVoiceChannel(
        SocketVoiceChannel? channel,
        IReadOnlyCollection<GuildCreateChannelDto>? guildCreateChannels)
    {
        if (channel is null) return false;
        if (guildCreateChannels is null) return false;
        return guildCreateChannels
            .Select(x => x.ChannelId)
            .Contains((long) channel.Id);
    }

    private static bool WasInCreatedChannelsCategory(
        SocketVoiceChannel? channel,
        IReadOnlyCollection<GuildCreateChannelDto>? guildCreateChannels)
    {
        if (channel is null) return false;
        if (guildCreateChannels is null) return false;
        return channel.CategoryId is not null &&
               guildCreateChannels
                   .Select(x => x.CategoryId)
                   .Contains((long) channel.CategoryId) &&
               guildCreateChannels
                   .Select(x => x.ChannelId)
                   .Contains((long) channel.Id) is false &&
               channel.ConnectedUsers.Count == 0;
    }
}
