using System.Threading.Tasks;
using Discord.Interactions;

namespace Vox.Services.Discord.Interactions.Modals;

public class SendMessageModalRespond : InteractionModuleBase<SocketInteractionContext>
{
    [ModalInteraction("send-message-modal:*")]
    public async Task Execute(string channelId, SendMessageModal modal)
    {
        await DeferAsync(true);

        var channel = Context.Guild.GetTextChannel(ulong.Parse(channelId));

        await channel.SendMessageAsync(modal.MessageText);
        await FollowupAsync($"Message sent successfully to {channel.Mention}.", ephemeral: true);
    }
}