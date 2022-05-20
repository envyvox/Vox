using System.Threading.Tasks;
using Discord.Interactions;
using MediatR;
using Newtonsoft.Json;
using Vox.Services.Discord.Embed;
using Vox.Services.Discord.Embed.Models;

namespace Vox.Services.Discord.Interactions.Modals;

public class SendEmbedModalRespond : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public SendEmbedModalRespond(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ModalInteraction("send-embed-modal:*")]
    public async Task Execute(string channelId, SendEmbedModal modal)
    {
        await DeferAsync(true);

        var channel = Context.Guild.GetTextChannel(ulong.Parse(channelId));
        var embedModel = JsonConvert.DeserializeObject<EmbedModel>(modal.JsonCode)!;
        var embed = await _mediator.Send(new BuildEmbedFromEmbedModel(embedModel));

        await channel.SendMessageAsync(embedModel.PlainText, embed: embed);
        await FollowupAsync($"Embed sent successfully to {channel.Mention}.", ephemeral: true);
    }
}