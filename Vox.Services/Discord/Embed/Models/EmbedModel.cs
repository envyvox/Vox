﻿using System.Text.Json.Serialization;

namespace Vox.Services.Discord.Embed.Models;

public class EmbedModel
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("author")] public EmbedAuthor? Author { get; set; }
    [JsonPropertyName("color")] public string? Color { get; set; }
    [JsonPropertyName("footer")] public EmbedFooter? Footer { get; set; }
    [JsonPropertyName("thumbnail")] public string? Thumbnail { get; set; }
    [JsonPropertyName("image")] public string? Image { get; set; }
    [JsonPropertyName("fields")] public EmbedField[]? Fields { get; set; }
}