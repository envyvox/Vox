using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using MediatR;
using Vox.Services.Discord.Client;

namespace Vox.Services.Discord.Guild.Queries;

public record GetSocketGuildQuery(ulong Id) : IRequest<SocketGuild>;

public class GetSocketGuildHandler : IRequestHandler<GetSocketGuildQuery, SocketGuild>
{
    private readonly IDiscordClientService _discordClientService;

    public GetSocketGuildHandler(IDiscordClientService discordClientService)
    {
        _discordClientService = discordClientService;
    }

    public async Task<SocketGuild> Handle(GetSocketGuildQuery request, CancellationToken ct)
    {
        var client = await _discordClientService.GetSocketClient();
        var guild = client.GetGuild(request.Id);

        if (guild is null)
        {
            throw new Exception(
                $"guild {request.Id} not found");
        }

        return guild;
    }
}