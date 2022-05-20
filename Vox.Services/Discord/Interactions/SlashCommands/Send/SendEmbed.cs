using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Services.Discord.Interactions.Modals;

namespace Vox.Services.Discord.Interactions.SlashCommands.Send;

[RequireUserPermission(GuildPermission.ManageMessages)]
public class SendEmbed : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("send-embed", "Send a embed to the specified channel")]
    public async Task Execute(
        [Summary("channel", "Text channel")] ITextChannel channel)
    {
        await RespondWithModalAsync<SendEmbedModal>($"send-embed-modal:{channel.Id}");
    }
}