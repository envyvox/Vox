using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Services.Discord.Interactions.Modals;

namespace Vox.Services.Discord.Interactions.SlashCommands.Send;

[RequireUserPermission(GuildPermission.ManageMessages)]
public class SendMessage : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("send-message", "Send a message to the specified channel")]
    public async Task Execute(
        [Summary("channel", "Text channel")] ITextChannel channel)
    {
        await RespondWithModalAsync<SendMessageModal>($"send-message-modal:{channel.Id}");
    }
}