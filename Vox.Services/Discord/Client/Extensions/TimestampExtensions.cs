using System;

namespace Vox.Services.Discord.Client.Extensions;

public enum TimestampFormat : byte
{
    /// <summary>
    /// 't:' Short time (e.g. 9:41 PM)
    /// </summary>
    ShortTime,

    /// <summary>
    /// 'T:' Long time (e.g. 9:41:30 PM)
    /// </summary>
    LongTime,

    /// <summary>
    /// 'd:' Short date (e.g. 30/06/2021)
    /// </summary>
    ShortDate,

    /// <summary>
    /// 'D:' Long date (e.g. 30 June 2021)
    /// </summary>
    LongDate,

    /// <summary>
    /// 'f' (default) Short date/time (e.g. 30 June 2021 9:41 PM)
    /// </summary>
    ShortDateTime,

    /// <summary>
    /// 'F:' Long date/time (e.g. Wednesday, June, 30, 2021 9:41 PM)
    /// </summary>
    LongDateTime,

    /// <summary>
    /// 'R:' Relative time (e.g. 2 months ago, in an hour)
    /// </summary>
    RelativeTime
}

public static class TimestampExtensions
{
    public static string ConvertToDiscordTimestamp(this DateTimeOffset dateTime, TimestampFormat format)
    {
        return $"<t:{dateTime.ToUnixTimeSeconds()}:{format.Flag()}>";
    }

    private static string Flag(this TimestampFormat format)
    {
        return format switch
        {
            TimestampFormat.ShortTime => "t",
            TimestampFormat.LongTime => "T",
            TimestampFormat.ShortDate => "d",
            TimestampFormat.LongDate => "D",
            TimestampFormat.ShortDateTime => "f",
            TimestampFormat.LongDateTime => "F",
            TimestampFormat.RelativeTime => "R",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}
