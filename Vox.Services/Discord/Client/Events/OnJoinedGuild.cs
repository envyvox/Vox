using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Vox.Services.Guild.Commands;

namespace Vox.Services.Discord.Client.Events;

public record OnJoinedGuild(SocketGuild SocketGuild) : IRequest;

public class OnJoinedGuildHandler : IRequestHandler<OnJoinedGuild>
{
    private readonly IMediator _mediator;

    public OnJoinedGuildHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(OnJoinedGuild request, CancellationToken ct)
    {
        await _mediator.Send(new CreateGuildEntityCommand((long) request.SocketGuild.Id));

        return Unit.Value;
    }
}