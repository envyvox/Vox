using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Vox.Services.Guild.Commands;

namespace Vox.Services.Discord.Client.Events;

public record OnLeftGuild(SocketGuild SocketGuild) : IRequest;

public class OnLeftGuildHandler : IRequestHandler<OnLeftGuild>
{
    private readonly IMediator _mediator;

    public OnLeftGuildHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> Handle(OnLeftGuild request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteGuildEntityCommand((long) request.SocketGuild.Id));

        return Unit.Value;
    }
}