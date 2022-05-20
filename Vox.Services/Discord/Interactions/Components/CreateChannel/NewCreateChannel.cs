using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using MediatR;
using Vox.Data.Enums;
using Vox.Services.Discord.Emote.Extensions;
using Vox.Services.Discord.Extensions;
using Vox.Services.Extensions;
using Vox.Services.GuildCreateChannel.Commands;
using Vox.Services.GuildCreateChannel.Queries;
using static Vox.Services.Extensions.ExceptionExtensions;

namespace Vox.Services.Discord.Interactions.Components.CreateChannel;

public class NewCreateChannel : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IMediator _mediator;

    public NewCreateChannel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ComponentInteraction("new-create-channel")]
    public async Task Execute()
    {
        await DeferAsync(true);

        var emotes = DiscordRepository.Emotes;
        var category = await Context.Guild.CreateCategoryChannelAsync(
            Response.NewCreatedChannelCategoryName.Parse(Context.Guild.PreferredLocale));
        var channel = await Context.Guild.CreateVoiceChannelAsync(
            Response.NewCreatedChannelName.Parse(Context.Guild.PreferredLocale),
            x => x.CategoryId = category.Id);

        var guildCreateChannels = await _mediator.Send(new GetGuildCreateChannelsQuery((long) Context.Guild.Id));

        if (guildCreateChannels.Count >= 3)
        {
            throw new ExpectedException(Response.CreateChannelLimitation.Parse(Context.Guild.PreferredLocale));
        }

        await _mediator.Send(new CreateGuildCreateChannelCommand(
            (long) Context.Guild.Id, (long) category.Id, (long) channel.Id));

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.SettingsCreateChannelButtonCreate.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(
                Response.NewCreateChannelDesc.Parse(Context.Guild.PreferredLocale,
                    Context.User.Mention, channel.Mention, emotes.GetEmote("Text"), emotes.GetEmote("ID")));

        await FollowupAsync(embed: embed.Build(), ephemeral: true);
    }
}