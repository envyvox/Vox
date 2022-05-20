using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vox.Services.Discord.Client.Events;
using Vox.Services.Discord.Extensions;

namespace Vox.Services.Discord.Client;

public class DiscordClientService : IDiscordClientService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly DiscordClientOptions _options;

    private DiscordSocketClient _socketClient;
    private InteractionService _interactionService;

    public DiscordClientService(
        IServiceProvider serviceProvider,
        IMediator mediator,
        IOptions<DiscordClientOptions> options)
    {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _options = options.Value;
    }

    public async Task Start()
    {
        _socketClient = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 100,
            AlwaysDownloadUsers = true,
            GatewayIntents =
                GatewayIntents.Guilds |
                GatewayIntents.GuildMembers |
                GatewayIntents.GuildMessageReactions |
                GatewayIntents.GuildMessages |
                GatewayIntents.GuildVoiceStates |
                GatewayIntents.GuildScheduledEvents
        });
        _interactionService = new InteractionService(_socketClient.Rest, new InteractionServiceConfig
        {
            UseCompiledLambda = true
        });

        await _serviceProvider.GetRequiredService<CommandHandler>()
            .InitializeAsync(_socketClient, _interactionService, _serviceProvider);

        await _socketClient.LoginAsync(TokenType.Bot, _options.Token);
        await _socketClient.StartAsync();

        _socketClient.Log += ClientOnLog;
        _socketClient.Ready += ClientOnReady;
        _socketClient.JoinedGuild += ClientOnJoinedGuild;
        _socketClient.LeftGuild += ClientOnLeftGuild;
        _socketClient.UserVoiceStateUpdated += ClientOnUserVoiceStateUpdated;
        _interactionService.Log += ClientOnLog;
    }

    public async Task<DiscordSocketClient> GetSocketClient()
    {
        return await Task.FromResult(_socketClient);
    }

    private async Task ClientOnLog(LogMessage logMessage)
    {
        await _mediator.Send(new OnLog(logMessage));
    }

    private async Task ClientOnReady()
    {
        await _mediator.Send(new OnReady(_interactionService));
    }

    private async Task ClientOnJoinedGuild(SocketGuild socketGuild)
    {
        await _mediator.Send(new OnJoinedGuild(socketGuild));
    }

    private async Task ClientOnLeftGuild(SocketGuild socketGuild)
    {
        await _mediator.Send(new OnLeftGuild(socketGuild));
    }

    private async Task ClientOnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState oldSocketVoiceState,
        SocketVoiceState newSocketVoiceState)
    {
        await _mediator.Send(new OnUserVoiceStateUpdated(socketUser, oldSocketVoiceState, newSocketVoiceState));
    }
}