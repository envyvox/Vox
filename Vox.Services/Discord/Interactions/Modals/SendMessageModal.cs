using Discord;
using Discord.Interactions;

namespace Vox.Services.Discord.Interactions.Modals;

public class SendMessageModal : IModal
{
    public string Title => "Send message";
    
    [InputLabel("Message text")]
    [ModalTextInput("message-text", TextInputStyle.Paragraph, "Message text", maxLength: 2000)]
    public string MessageText { get; set; }
}