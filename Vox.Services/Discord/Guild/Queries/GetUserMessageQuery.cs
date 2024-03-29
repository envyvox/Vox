﻿using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;

namespace Vox.Services.Discord.Guild.Queries;

public record GetUserMessageQuery(ulong GuildId, ulong ChannelId, ulong MessageId) : IRequest<IUserMessage?>;

public class GetUserMessageHandler : IRequestHandler<GetUserMessageQuery, IUserMessage?>
{
    private readonly IMediator _mediator;

    public GetUserMessageHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IUserMessage?> Handle(GetUserMessageQuery request, CancellationToken ct)
    {
        var channel = await _mediator.Send(new GetSocketTextChannelQuery(request.GuildId, request.ChannelId));
        return (IUserMessage) await channel.GetMessageAsync(request.MessageId);
    }
}