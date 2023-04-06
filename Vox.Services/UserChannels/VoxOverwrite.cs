using System.Collections.Generic;
using System.Linq;
using Discord;

namespace Vox.Services.UserChannels;

public class VoxOverwrite
{
    public ulong TargetId { get; }
    public PermissionTarget TargetType { get; }
    public VoxOverwritePermissions Permissions { get; }

    public VoxOverwrite(ulong targetId, PermissionTarget targetType, VoxOverwritePermissions permissions)
    {
        TargetId = targetId;
        TargetType = targetType;
        Permissions = permissions;
    }

    public static IReadOnlyCollection<Overwrite> ToDiscordOverwrites(IReadOnlyCollection<VoxOverwrite> overwrites)
    {
        return overwrites.Select(overwrite => new Overwrite(
                overwrite.TargetId,
                overwrite.TargetType,
                new OverwritePermissions(overwrite.Permissions.AllowValue, overwrite.Permissions.DenyValue)))
            .ToList();
    }
}
