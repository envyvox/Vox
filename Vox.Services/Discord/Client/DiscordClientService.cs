using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vox.Framework.Autofac;
using Vox.Services.CreateChannels;
using Vox.Services.Discord.Emotes;
using Vox.Services.UserChannels;
using Emote = Vox.Services.Discord.Emotes.Emote;

namespace Vox.Services.Discord.Client;

[InjectableService(IsSingletone = true)]
public class DiscordClientService : IDiscordClientService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _env;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<DiscordClientService> _logger;
    private readonly ICreateChannelRepository _createChannelRepository;
    private readonly IUserChannelRepository _userChannelRepository;
    private readonly DiscordClientOptions _options;

    private DiscordSocketClient _socketClient;
    private InteractionService _interactionService;

    public DiscordClientService(
        IServiceProvider serviceProvider,
        IOptions<DiscordClientOptions> options,
        IHostEnvironment env,
        IHostApplicationLifetime lifetime,
        ILogger<DiscordClientService> logger,
        ICreateChannelRepository createChannelRepository,
        IUserChannelRepository userChannelRepository)
    {
        _serviceProvider = serviceProvider;
        _env = env;
        _lifetime = lifetime;
        _logger = logger;
        _createChannelRepository = createChannelRepository;
        _userChannelRepository = userChannelRepository;
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

    private Task ClientOnLog(LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                _logger.LogCritical("{Message}", logMessage.Message);
                break;
            case LogSeverity.Error:
                _logger.LogError("{Message}", logMessage.Message);
                break;
            case LogSeverity.Warning:
                _logger.LogWarning("{Message}", logMessage.Message);
                break;
            case LogSeverity.Info:
                _logger.LogInformation("{Message}", logMessage.Message);
                break;
            case LogSeverity.Verbose:
                _logger.LogInformation("{Message}", logMessage.Message);
                break;
            case LogSeverity.Debug:
                _logger.LogDebug("{Message}", logMessage.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return Task.CompletedTask;
    }

    private async Task ClientOnReady()
    {
        try
        {
            var socketClient = await GetSocketClient();
            var iconGuildsId = _options.IconGuildsId.Split(";").Select(ulong.Parse);

            foreach (var guild in socketClient.Guilds.Where(x => iconGuildsId.Contains(x.Id)))
            {
                foreach (var emote in guild.Emotes)
                {
                    if (EmoteRepository.Emotes.ContainsKey(emote.Name)) continue;

                    EmoteRepository.Emotes.Add(emote.Name, new Emote(emote.Id, emote.Name, emote.ToString()));
                }
            }

            _logger.LogInformation(
                "Emotes sync completed");

            if (_env.IsDevelopment())
            {
                _logger.LogInformation(
                    "Env is development, registering commands to guilds");

                await _interactionService.RegisterCommandsToGuildAsync(_options.TestingGuildId);
            }
            else
            {
                _logger.LogInformation(
                    "Env is production, registering commands globally");

                await _interactionService.RegisterCommandsGloballyAsync();
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

    private Task ClientOnJoinedGuild(SocketGuild socketGuild)
    {
        _logger.LogInformation("Joined guild {@Guild}", socketGuild);
        return Task.CompletedTask;
    }

    private Task ClientOnLeftGuild(SocketGuild socketGuild)
    {
        _logger.LogInformation("Left guild {@Guild}", socketGuild);
        return Task.CompletedTask;
    }

    private async Task ClientOnUserVoiceStateUpdated(SocketUser socketUser, SocketVoiceState oldSocketVoiceState,
        SocketVoiceState newSocketVoiceState)
    {
        var oldChannel = oldSocketVoiceState.VoiceChannel;
        var newChannel = newSocketVoiceState.VoiceChannel;
        var socketGuild = oldChannel is null ? newChannel.Guild : oldChannel.Guild;
        var guildCreateChannels = await _createChannelRepository.List((long) socketGuild.Id);

        // If this guild does not have "create channels" just return
        if (guildCreateChannels.Count < 1) return;

        var userChannel = await _userChannelRepository.Get(socketUser);

        if (IsCreateVoiceChannel(newChannel, guildCreateChannels))
        {
            var createdChannel = await socketGuild.CreateVoiceChannelAsync(userChannel.ChannelName, x =>
            {
                x.CategoryId = (ulong) guildCreateChannels
                    .Single(createChannel => createChannel.ChannelId == (long) newChannel.Id)
                    .CategoryId;
                x.UserLimit = userChannel.ChannelLimit;
            });

            await ((SocketGuildUser) socketUser).ModifyAsync(x => x.Channel = createdChannel);
            await ApplyPermissionsToChannel(createdChannel, socketGuild, VoxOverwrite.ToDiscordOverwrites(userChannel.Overwrites));
        }

        if (WasInCreatedChannelsCategory(oldChannel, guildCreateChannels))
        {
            userChannel.UpdateChannelName(oldChannel!.Name);
            userChannel.UpdateChannelLimit(oldChannel.UserLimit);
            userChannel.UpdateOverwrites(oldChannel.PermissionOverwrites);

            await _userChannelRepository.Save(userChannel);
            await oldChannel.DeleteAsync();
        }
    }

    private static async Task ApplyPermissionsToChannel(RestVoiceChannel? channel, SocketGuild? guild,
        IReadOnlyCollection<Overwrite>? overwrites)
    {
        if (channel is null)
        {
            throw new Exception("Rest voice channel is null");
        }

        if (guild is null)
        {
            throw new Exception("Socket guild is null");
        }

        if (overwrites is null)
        {
            throw new Exception("Error parsing permissions");
        }

        foreach (var overwrite in overwrites)
        {
            switch (overwrite.TargetType)
            {
                case PermissionTarget.Role:
                    var role = guild.GetRole(overwrite.TargetId);
                    await channel.AddPermissionOverwriteAsync(role, overwrite.Permissions);
                    break;
                case PermissionTarget.User:
                    var user = guild.GetUser(overwrite.TargetId);
                    await channel.AddPermissionOverwriteAsync(user, overwrite.Permissions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static bool IsCreateVoiceChannel(
        SocketVoiceChannel? channel,
        IReadOnlyCollection<CreateChannel>? guildCreateChannels)
    {
        if (channel is null) return false;
        if (guildCreateChannels is null) return false;
        return guildCreateChannels
            .Select(x => x.ChannelId)
            .Contains((long) channel.Id);
    }

    private static bool WasInCreatedChannelsCategory(
        SocketVoiceChannel? channel,
        IReadOnlyCollection<CreateChannel>? guildCreateChannels)
    {
        if (channel is null) return false;
        if (guildCreateChannels is null) return false;
        return channel.CategoryId is not null &&
               guildCreateChannels
                   .Select(x => x.CategoryId)
                   .Contains((long) channel.CategoryId) &&
               guildCreateChannels
                   .Select(x => x.ChannelId)
                   .Contains((long) channel.Id) is false &&
               channel.ConnectedUsers.Count == 0;
    }
}
