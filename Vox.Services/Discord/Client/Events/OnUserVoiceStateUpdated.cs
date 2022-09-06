using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MediatR;
using Vox.Services.Guild.Queries;
using Vox.Services.GuildCreateChannel.Queries;

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

    public async Task<Unit> Handle(OnUserVoiceStateUpdated request, CancellationToken ct)
    {
        var oldChannel = request.OldSocketVoiceState.VoiceChannel;
        var newChannel = request.NewSocketVoiceState.VoiceChannel;
        var socketGuild = oldChannel is null ? newChannel.Guild : oldChannel.Guild;
        var guild = await _mediator.Send(new GetGuildEntityQuery((long) socketGuild.Id));
        var guildCreateChannels = await _mediator.Send(new GetGuildCreateChannelsQuery(guild.Id));

        if (guildCreateChannels.Count < 1) return Unit.Value;

        if (newChannel is not null &&
            guildCreateChannels
                .Select(x => x.ChannelId)
                .Contains((long) newChannel.Id))
        {
            var categoryId = (ulong) guildCreateChannels
                .Single(x => x.ChannelId == (long) newChannel.Id)
                .CategoryId;

            var createdChannel = await socketGuild.CreateVoiceChannelAsync(request.SocketUser.Username, x =>
            {
                x.CategoryId = categoryId;
                x.UserLimit = guild.CreateRoomLimit;
            });

            await ((SocketGuildUser) request.SocketUser).ModifyAsync(x => x.Channel = createdChannel);

            await createdChannel.AddPermissionOverwriteAsync(request.SocketUser, new OverwritePermissions(
                manageChannel: PermValue.Allow,
                moveMembers: PermValue.Allow,
                manageRoles: PermValue.Allow));
        }

        if (oldChannel?.CategoryId is not null &&
            guildCreateChannels
                .Select(x => x.CategoryId)
                .Contains((long) oldChannel.CategoryId) &&
            guildCreateChannels
                .Select(x => x.ChannelId)
                .Contains((long) oldChannel.Id) is false &&
            oldChannel.ConnectedUsers.Count == 0)
        {
            await oldChannel.DeleteAsync();
        }

        return Unit.Value;
    }
}