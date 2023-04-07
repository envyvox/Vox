using System.Collections.Generic;

namespace Vox.Services.GTP;

// ReSharper disable once InconsistentNaming
public static class GPTRepository
{
    public static readonly Dictionary<ulong, List<Message>> UserMessages = new();
}
