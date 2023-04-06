using System;
using System.Text.Json.Serialization;
using Discord;

namespace Vox.Services.Discord.Embed;

public class EmbedModel
{
    [JsonPropertyName("plainText")] public string? PlainText { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("author")] public EmbedAuthor? Author { get; set; }
    [JsonPropertyName("color")] public long? Color { get; set; }
    [JsonPropertyName("footer")] public EmbedFooter? Footer { get; set; }
    [JsonPropertyName("thumbnail")] public string? Thumbnail { get; set; }
    [JsonPropertyName("image")] public string? Image { get; set; }
    [JsonPropertyName("fields")] public EmbedField[]? Fields { get; set; }

    public global::Discord.Embed BuildEmbed()
    {
        var embed = new EmbedBuilder()
            .WithColor(new Color(Convert.ToUInt32(Color)));

        if (Title is not null)
            embed.WithTitle(Title);
        if (Description is not null)
            embed.WithDescription(Description);
        if (Author is not null)
            embed.WithAuthor(Author.Name, Author.IconUrl, Author.Url);
        if (Footer is not null)
            embed.WithFooter(Footer.Text, Footer.IconUrl);
        if (Thumbnail is not null)
            embed.WithThumbnailUrl(Thumbnail);
        if (Image is not null)
            embed.WithImageUrl(Image);

        if (Fields == null) return embed.Build();

        foreach (var field in Fields)
            embed.AddField(field.Name, field.Value, field.Inline);

        return embed.Build();
    }
}
