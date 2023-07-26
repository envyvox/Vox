using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;
using Vox.Services.Discord.Embed.Models;
using Vox.Services.Discord.Extensions;

namespace Vox.Services.Discord.Embed;

public record BuildEmbedsFromModels(EmbedModel[]? Models)
    : IRequest<global::Discord.Embed[]>;

public class BuildEmbedsFromModelsHandler
    : IRequestHandler<BuildEmbedsFromModels, global::Discord.Embed[]>
{
    public Task<global::Discord.Embed[]> Handle(BuildEmbedsFromModels request,
        CancellationToken ct)
    {
        var embeds = new List<global::Discord.Embed>();

        if (request.Models is null) return Task.FromResult(embeds.ToArray());

        foreach (var model in request.Models)
        {
            var embed = new EmbedBuilder();

            if (model.Color is null) embed.WithDefaultColor();
            else embed.WithColor(new Color(uint.Parse(model.Color.Replace("#", ""), NumberStyles.HexNumber)));

            if (model.Title is not null)
                embed.WithTitle(model.Title);
            if (model.Description is not null)
                embed.WithDescription(model.Description);
            if (model.Author is not null)
                embed.WithAuthor(model.Author.Name, model.Author.IconUrl, model.Author.Url);
            if (model.Footer is not null)
                embed.WithFooter(model.Footer.Text, model.Footer.IconUrl);
            if (model.Thumbnail is not null)
                embed.WithThumbnailUrl(model.Thumbnail);
            if (model.Image is not null)
                embed.WithImageUrl(model.Image);

            if (model.Fields == null)
            {
                embeds.Add(embed.Build());
                continue;
            }

            foreach (var field in model.Fields)
                embed.AddField(field.Name, field.Value, field.Inline);

            embeds.Add(embed.Build());
        }

        return Task.FromResult(embeds.ToArray());
    }
}