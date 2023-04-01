using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Emote.Models;
using Vox.Services.Discord.Extensions;
using Vox.Services.Extensions;

namespace Vox.Services.Discord.Emote.Commands
{
    public record SyncEmotesCommand : IRequest;

    public class SyncEmotesHandler : IRequestHandler<SyncEmotesCommand>
    {
        private readonly IDiscordClientService _discordClientService;
        private readonly ILogger<SyncEmotesHandler> _logger;
        private readonly DiscordClientOptions _options;

        public SyncEmotesHandler(
            IDiscordClientService discordClientService,
            ILogger<SyncEmotesHandler> logger,
            IOptions<DiscordClientOptions> options)
        {
            _discordClientService = discordClientService;
            _logger = logger;
            _options = options.Value;
        }

        public async Task Handle(SyncEmotesCommand request, CancellationToken ct)
        {
            var socketClient = await _discordClientService.GetSocketClient();
            var emotes = DiscordRepository.Emotes;
            var iconGuildsId = _options.IconGuildsId.Split(";").Select(ulong.Parse);

            foreach (var guild in socketClient.Guilds.Where(x => iconGuildsId.Contains(x.Id)))
            {
                foreach (var emote in guild.Emotes)
                {
                    if (emotes.ContainsKey(emote.Name)) continue;

                    emotes.Add(emote.Name, new EmoteDto(emote.Id, emote.Name, emote.ToString()));
                }
            }

            _logger.LogInformation(
                "Emotes sync completed");
        }
    }
}
