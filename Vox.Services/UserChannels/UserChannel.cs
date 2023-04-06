using System;
using System.Collections.Generic;
using System.Linq;
using Discord;

namespace Vox.Services.UserChannels;

public class UserChannel
{
    public Guid Id { get; }
    public long UserId { get; }
    public string ChannelName { get; private set; }
    public int ChannelLimit { get; private set; }
    public IReadOnlyCollection<VoxOverwrite> Overwrites { get; private set; }

    public UserChannel(Guid id, long userId, string channelName, int channelLimit,
        IReadOnlyCollection<VoxOverwrite> overwrites)
    {
        Id = id;
        UserId = userId;
        ChannelName = channelName;
        ChannelLimit = channelLimit;
        Overwrites = overwrites;
    }

    public void UpdateChannelName(string channelName)
    {
        ChannelName = channelName;
    }

    public void UpdateChannelLimit(int? channelLimit)
    {
        ChannelLimit = channelLimit ?? 0;
    }

    public void UpdateOverwrites(IReadOnlyCollection<Overwrite> overwrites)
    {
        Overwrites = overwrites.Select(overwrite => new VoxOverwrite(
                overwrite.TargetId,
                overwrite.TargetType,
                new VoxOverwritePermissions(overwrite.Permissions.AllowValue, overwrite.Permissions.DenyValue)))
            .ToList();
    }
}
