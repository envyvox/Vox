using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.CreateChannels;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Client.Extensions;
using Vox.Services.Discord.Emotes;
using static Discord.Emote;

namespace Vox.Services.Discord.Interactions.Components.CreateChannel;

public class DeleteCreateChannel : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICreateChannelRepository _createChannelRepository;


    public DeleteCreateChannel(ICreateChannelRepository createChannelRepository)
    {
        _createChannelRepository = createChannelRepository;
    }

    [ComponentInteraction("delete-create-channel")]
    public async Task Execute()
    {
        await DeferAsync(true);

        var emotes = EmoteRepository.Emotes;
        var guildCreateChannels = await _createChannelRepository.List((long) Context.Guild.Id);

        var selectMenu = new SelectMenuBuilder()
            .WithPlaceholder(Response.DeleteCreateChannelPlaceholder.Parse(Context.Guild.PreferredLocale))
            .WithCustomId("delete-create-channel-selected")
            .WithMaxValues(guildCreateChannels.Count);

        foreach (var createChannel in guildCreateChannels)
        {
            var category = Context.Guild.GetCategoryChannel((ulong) createChannel.CategoryId);
            var channel = Context.Guild.GetVoiceChannel((ulong) createChannel.ChannelId);

            selectMenu.AddOption(
                Response.DeleteCreateChannelOption.Parse(Context.Guild.PreferredLocale,
                    channel.Name, category.Name),
                $"{channel.Id},{category.Id}",
                emote: Parse(emotes.GetEmote("VoiceChannel")));
        }

        await ModifyOriginalResponseAsync(x =>
        {
            x.Content = StringExtensions.EmptyChar;
            x.Components = new ComponentBuilder().WithSelectMenu(selectMenu).Build();
        });
    }
}
