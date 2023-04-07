using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Vox.Services.Discord.Client.Events;

public record OnLog(LogMessage LogMessage) : IRequest;

public class OnLogHandler : IRequestHandler<OnLog>
{
    private readonly ILogger<OnLogHandler> _logger;

    public OnLogHandler(ILogger<OnLogHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OnLog request, CancellationToken ct)
    {
        switch (request.LogMessage.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical("{Message}", request.LogMessage.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError("{Message}", request.LogMessage.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("{Message}", request.LogMessage.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{Message}", request.LogMessage.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogInformation("{Message}", request.LogMessage.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug("{Message}", request.LogMessage.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Task.CompletedTask;
    }
}
