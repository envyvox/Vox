using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vox.Services.Discord.Emote.Commands;
using Vox.Services.Discord.Extensions;

namespace Vox.Services.Discord.Client.Events;

public record OnReady(InteractionService InteractionService) : IRequest;

public class OnReadyHandler : IRequestHandler<OnReady>
{
    private readonly ILogger<OnReadyHandler> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IMediator _mediator;
    private readonly IHostEnvironment _env;
    private readonly DiscordClientOptions _options;

    public OnReadyHandler(
        IOptions<DiscordClientOptions> options,
        ILogger<OnReadyHandler> logger,
        IHostApplicationLifetime lifetime,
        IMediator mediator,
        IHostEnvironment env)
    {
        _options = options.Value;
        _logger = logger;
        _lifetime = lifetime;
        _mediator = mediator;
        _env = env;
    }

    public async Task Handle(OnReady request, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new SyncEmotesCommand());

            if (_env.IsDevelopment())
            {
                _logger.LogInformation(
                    "Env is development, registering commands to guilds");

                await request.InteractionService.RegisterCommandsToGuildAsync(_options.TestingGuildId);
            }
            else
            {
                _logger.LogInformation(
                    "Env is production, registering commands globally");

                await request.InteractionService.RegisterCommandsGloballyAsync();
            }

            _logger.LogInformation(
                "Bot started");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e,
                "Unable to startup the bot. Application will now exit");

            _lifetime.StopApplication();
        }
    }
}
