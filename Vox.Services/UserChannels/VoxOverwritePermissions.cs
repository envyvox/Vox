namespace Vox.Services.UserChannels;

public class VoxOverwritePermissions
{
    public ulong AllowValue { get; }
    public ulong DenyValue { get; }


    public VoxOverwritePermissions(ulong allowValue, ulong denyValue)
    {
        AllowValue = allowValue;
        DenyValue = denyValue;
    }
}
