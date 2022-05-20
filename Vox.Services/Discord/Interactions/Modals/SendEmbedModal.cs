using Discord;
using Discord.Interactions;

namespace Vox.Services.Discord.Interactions.Modals;

public class SendEmbedModal : IModal
{
    public string Title => "Send embed";

    [InputLabel("Generated JSON code")]
    [ModalTextInput("json-code", TextInputStyle.Paragraph, "Generated JSON code from oldeb.nadeko.bot")]
    public string JsonCode { get; set; }
}