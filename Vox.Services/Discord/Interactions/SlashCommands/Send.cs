using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using MediatR;
using Newtonsoft.Json;
using Vox.Data.Enums;
using Vox.Services.Discord.Embed;
using Vox.Services.Discord.Embed.Models;
using static Vox.Services.Extensions.ExceptionExtensions;

namespace Vox.Services.Discord.Interactions.SlashCommands;

[RequireUserPermission(GuildPermission.ManageMessages)]
public class Send : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public Send(IMediator mediator)
    {
        _mediator = mediator;
    }

    public enum MessageType
    {
        Message = 1,
        Embed = 2
    }

    [SlashCommand("send", "Send a message or embed")]
    public async Task Execute(
        [Summary("type", "Plain text or embed")]
        MessageType type,
        [Summary("message", "Plain text or embed Json (Use eb.nadeko.bot website to generate Json)")]
        string message,
        [Summary("channel", "Text channel")] ITextChannel? channel = null)
    {
        await DeferAsync(true);

        var chan = channel ?? Context.Channel as ITextChannel;

        if (chan is null)
        {
            throw new ExpectedException(Response.NullChannel.Parse(Context.Guild.PreferredLocale));
        }

        switch (type)
        {
            case MessageType.Message:
            {
                await chan.SendMessageAsync(message);
                break;
            }
            case MessageType.Embed:
            {
                var messageModel = JsonConvert.DeserializeObject<MessageModel>(message);

                if (messageModel is null)
                {
                    throw new ExpectedException(Response.SendEmbedInvalidJson.Parse(Context.Guild.PreferredLocale));
                }

                var embeds = await _mediator.Send(new BuildEmbedsFromModels(messageModel.Embeds));

                await chan.SendMessageAsync(messageModel.Content, embeds: embeds);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        await FollowupAsync($"Message sent successfully to {chan.Mention}.", ephemeral: true);
    }
}