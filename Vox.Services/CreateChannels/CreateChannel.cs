using System;

namespace Vox.Services.CreateChannels;

public class CreateChannel
{
    public Guid Id { get; }
    public long GuildId { get; }
    public long CategoryId { get; }
    public long ChannelId { get; }

    public CreateChannel(Guid id, long guildId, long categoryId, long channelId)
    {
        Id = id;
        GuildId = guildId;
        CategoryId = categoryId;
        ChannelId = channelId;
    }
}
