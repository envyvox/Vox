using System.Text.Json;
using System.Threading.Tasks;
using Discord.Interactions;
using Vox.Services.Discord.Embed;

namespace Vox.Services.Discord.Interactions.Modals;

public class SendEmbedModalRespond : InteractionModuleBase<SocketInteractionContext>
{
    [ModalInteraction("send-embed-modal:*")]
    public async Task Execute(string channelId, SendEmbedModal modal)
    {
        await DeferAsync(true);

        var channel = Context.Guild.GetTextChannel(ulong.Parse(channelId));
        var embedModel = JsonSerializer.Deserialize<EmbedModel>(modal.JsonCode,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        await channel.SendMessageAsync(embedModel.PlainText, embed: embedModel.BuildEmbed());
        await FollowupAsync($"Embed sent successfully to {channel.Mention}.", ephemeral: true);
    }
}
