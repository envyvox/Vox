using System.Text.Json.Serialization;

namespace Vox.Services.Discord.Embed.Models;

public class EmbedField
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("value")] public string Value { get; set; }
    [JsonPropertyName("inline")] public bool Inline { get; set; }
}