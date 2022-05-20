using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;

namespace Vox.Services.Discord.Guild.Queries;

public record GetSocketTextChannelQuery(ulong GuildId, ulong ChannelId) : IRequest<SocketTextChannel>;

public class GetSocketTextChannelHandler : IRequestHandler<GetSocketTextChannelQuery, SocketTextChannel>
{
    private readonly IMediator _mediator;

    public GetSocketTextChannelHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<SocketTextChannel> Handle(GetSocketTextChannelQuery request, CancellationToken ct)
    {
        var guild = await _mediator.Send(new GetSocketGuildQuery(request.GuildId));
        var channel = guild.GetTextChannel(request.ChannelId);

        if (channel is null)
        {
            throw new Exception(
                $"channel {request.ChannelId} not found in guild {request.GuildId}");
        }

        return channel;
    }
}