﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Vox.Data.Enums;
using Vox.Services.Discord.Extensions;
using static Vox.Services.Extensions.ExceptionExtensions;

namespace Vox.Services.Discord.Client;

public class CommandHandler
{
    private readonly ILogger<CommandHandler> _logger;

    private DiscordSocketClient _client;
    private InteractionService _commands;
    private IServiceProvider _services;

    public CommandHandler(ILogger<CommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task InitializeAsync(DiscordSocketClient discordSocketClient,
        InteractionService interactionService, IServiceProvider serviceProvider)
    {
        _client = discordSocketClient;
        _commands = interactionService;
        _services = serviceProvider;

        await _commands.AddModulesAsync(typeof(IDiscordClientService).Assembly, _services);

        _client.InteractionCreated += HandleInteraction;
        _commands.SlashCommandExecuted += SlashCommandExecuted;
        _commands.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private async Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo,
        IInteractionContext interactionContext, IResult result)
    {
        var componentCmd = (SocketMessageComponent) interactionContext.Interaction;

        _logger.LogInformation(
            "User {Username} ({UserId}) executed a component with id {CustomId}",
            componentCmd.User.Username, componentCmd.User.Id, componentCmd.Data.CustomId);

        if (result.IsSuccess is false)
        {
            if (result.Error is InteractionCommandError.Exception)
            {
                await HandleExceptions(componentCmd, (ExecuteResult) result);
            }
            else
            {
                await componentCmd.DeferAsync(true);
                await HandleError(componentCmd, result);
            }
        }
        else await Task.CompletedTask;
    }

    private async Task SlashCommandExecuted(SlashCommandInfo slashCommandInfo,
        IInteractionContext interactionContext, IResult result)
    {
        var slashCmd = (SocketSlashCommand) interactionContext.Interaction;

        _logger.LogInformation(
            "User {Username} ({UserId}) executed a slash command /{SlashCommandName}",
            slashCmd.User.Username, slashCmd.User.Id, slashCmd.Data.Name);

        if (result.IsSuccess is false)
        {
            if (result.Error is InteractionCommandError.Exception)
            {
                await HandleExceptions(slashCmd, (ExecuteResult) result);
            }
            else
            {
                await slashCmd.DeferAsync(true);
                await HandleError(slashCmd, result);
            }
        }

        else await Task.CompletedTask;
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new SocketInteractionContext(_client, socketInteraction);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception)
        {
            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (socketInteraction.Type == InteractionType.ApplicationCommand)
                await socketInteraction.GetOriginalResponseAsync()
                    .ContinueWith(async msg => await msg.Result.DeleteAsync());
        }
    }

    private async Task HandleExceptions(IDiscordInteraction interaction, ExecuteResult result)
    {
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(Response.SomethingWentWrongTitle.Parse(interaction.GuildLocale),
                interaction.User.GetAvatarUrl());

        switch (result.Exception)
        {
            case ExpectedException expectedException:
            {
                embed.WithDescription($"{interaction.User.Mention}, {expectedException.Message}.");

                break;
            }

            default:
            {
                embed.WithDescription(Response.SomethingWentWrongDesc.Parse(interaction.GuildLocale,
                    interaction.User.Mention));

                _logger.LogError(result.Exception, "Interaction ended with unexpected exception");

                break;
            }
        }

        await interaction.FollowupAsync(embed: embed.Build(), ephemeral: true);
    }

    private static async Task HandleError(IDiscordInteraction interaction, IResult result)
    {
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(Response.SomethingWentWrongTitle.Parse(interaction.GuildLocale),
                interaction.User.GetAvatarUrl())
            .WithDescription($"{interaction.User.Mention}, {result.ErrorReason}.");

        await interaction.FollowupAsync(embed: embed.Build(), ephemeral: true);
    }
}