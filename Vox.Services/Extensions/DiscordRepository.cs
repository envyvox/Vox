using System.Collections.Generic;
using Vox.Services.Discord.Emote.Models;

namespace Vox.Services.Extensions;

public static class DiscordRepository
{
    public static readonly Dictionary<string, EmoteDto> Emotes = new();
}