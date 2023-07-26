using System.Text.Json.Serialization;

namespace Vox.Services.Discord.Embed.Models;

public class MessageModel
{
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("embeds")] public EmbedModel[]? Embeds { get; set; }
}