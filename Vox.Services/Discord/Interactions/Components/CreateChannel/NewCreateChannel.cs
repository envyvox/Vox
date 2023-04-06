using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.CreateChannels;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Client.Extensions;
using Vox.Services.Discord.Embed;
using Vox.Services.Discord.Emotes;

namespace Vox.Services.Discord.Interactions.Components.CreateChannel;

public class NewCreateChannel : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICreateChannelRepository _createChannelRepository;

    public NewCreateChannel(ICreateChannelRepository createChannelRepository)
    {
        _createChannelRepository = createChannelRepository;
    }

    [ComponentInteraction("new-create-channel")]
    public async Task Execute()
    {
        await DeferAsync(true);

        var emotes = EmoteRepository.Emotes;
        var category = await Context.Guild.CreateCategoryChannelAsync(
            Response.NewCreatedChannelCategoryName.Parse(Context.Guild.PreferredLocale));
        var channel = await Context.Guild.CreateVoiceChannelAsync(
            Response.NewCreatedChannelName.Parse(Context.Guild.PreferredLocale),
            x => x.CategoryId = category.Id);

        var guildCreateChannels = await _createChannelRepository.List((long) Context.Guild.Id);

        if (guildCreateChannels.Count >= 3)
        {
            throw new ExceptionExtensions.ExpectedException(
                Response.CreateChannelLimitation.Parse(Context.Guild.PreferredLocale));
        }

        await _createChannelRepository.Create(new CreateChannels.CreateChannel(
            Guid.NewGuid(),
            (long) Context.Guild.Id,
            (long) category.Id,
            (long) channel.Id));

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.SettingsCreateChannelButtonCreate.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(
                Response.NewCreateChannelDesc.Parse(Context.Guild.PreferredLocale,
                    Context.User.Mention, channel.Mention, emotes.GetEmote("Text"), emotes.GetEmote("ID")));

        await ModifyOriginalResponseAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = new ComponentBuilder().Build();
        });
    }
}
