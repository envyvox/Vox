using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.CreateChannels;
using Vox.Services.Discord.Embed;

namespace Vox.Services.Discord.Interactions.Components.CreateChannel;

public class DeleteCreateChannelSelected : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ICreateChannelRepository _createChannelRepository;

    /// <inheritdoc />
    public DeleteCreateChannelSelected(ICreateChannelRepository createChannelRepository)
    {
        _createChannelRepository = createChannelRepository;
    }

    [ComponentInteraction("delete-create-channel-selected")]
    public async Task Execute(string[] selectedValues)
    {
        await DeferAsync(true);

        foreach (var selectedValue in selectedValues)
        {
            var value = selectedValue.Split(',').Select(ulong.Parse).ToArray();
            var channel = Context.Guild.GetVoiceChannel(value[0]);
            var category = Context.Guild.GetCategoryChannel(value[1]);

            await channel.DeleteAsync();
            await category.DeleteAsync();
            await _createChannelRepository.Delete((long) Context.Guild.Id, (long) category.Id);
        }

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.SettingsCreateChannelButtonDelete.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(Response.DeleteCreateChannelSelectedDesc.Parse(Context.Guild.PreferredLocale,
                Context.User.Mention));

        await ModifyOriginalResponseAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = new ComponentBuilder().Build();
        });
    }
}
