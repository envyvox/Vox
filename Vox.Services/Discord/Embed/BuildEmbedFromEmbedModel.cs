using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using MediatR;
using Vox.Services.Discord.Embed.Models;

namespace Vox.Services.Discord.Embed;

public record BuildEmbedFromEmbedModel(EmbedModel Model) : IRequest<global::Discord.Embed>;

public class BuildEmbedFromEmbedModelHandler : IRequestHandler<BuildEmbedFromEmbedModel, global::Discord.Embed>
{
    public async Task<global::Discord.Embed> Handle(BuildEmbedFromEmbedModel request, CancellationToken ct)
    {
        var embed = new EmbedBuilder()
            .WithColor(new Color(Convert.ToUInt32(request.Model.Color)));

        if (request.Model.Title is not null)
            embed.WithTitle(request.Model.Title);
        if (request.Model.Description is not null)
            embed.WithDescription(request.Model.Description);
        if (request.Model.Author is not null)
            embed.WithAuthor(request.Model.Author.Name, request.Model.Author.IconUrl, request.Model.Author.Url);
        if (request.Model.Footer is not null)
            embed.WithFooter(request.Model.Footer.Text, request.Model.Footer.IconUrl);
        if (request.Model.Thumbnail is not null)
            embed.WithThumbnailUrl(request.Model.Thumbnail);
        if (request.Model.Image is not null)
            embed.WithImageUrl(request.Model.Image);

        if (request.Model.Fields == null)
            return await Task.FromResult(embed.Build());

        foreach (var field in request.Model.Fields)
            embed.AddField(field.Name, field.Value, field.Inline);

        return await Task.FromResult(embed.Build());
    }
}