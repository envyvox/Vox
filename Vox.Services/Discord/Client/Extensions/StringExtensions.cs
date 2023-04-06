using System;
using System.Collections.Generic;
using Vox.Data.Enums;

namespace Vox.Services.Discord.Client.Extensions;

public static class StringExtensions
{
    /// <summary> Unicode Character “⠀” (U+2800) </summary>
    public const string EmptyChar = "⠀";

    public const string PageImageUrl =
        "https://cdn.discordapp.com/attachments/955683860287479858/955690421168799825/document.png";

    public static string RemoveFromEnd(this string source, int amount)
    {
        return source.Length < amount ? source : source.Remove(source.Length - amount);
    }

    public static string Localize(this string word, string preferredLocale, int amount)
    {
        var dict = preferredLocale.GetLanguageFromPreferredLocale() switch
        {
            Language.English => PluralizeEngDict,
            Language.Russian => PluralizeRuDict,
            _ => throw new ArgumentOutOfRangeException()
        };

        var n = Math.Abs(amount);

        n %= 100;
        if (n is >= 5 and <= 20) return dict[word].Value;

        n %= 10;
        if (n == 1) return word;
        if (n is >= 2 and <= 4) return dict[word].Key;

        return dict[word].Value;
    }

    private static readonly Dictionary<string, KeyValuePair<string, string>> PluralizeEngDict = new()
    {
        {
            "user", new KeyValuePair<string, string>("users", "users")
        },
        {
            "bot", new KeyValuePair<string, string>("bots", "bots")
        },
        {
            "role", new KeyValuePair<string, string>("roles", "roles")
        },
        {
            "channel", new KeyValuePair<string, string>("channels", "channels")
        },
        {
            "minute", new KeyValuePair<string, string>("minutes", "minutes")
        },
        {
            "answer", new KeyValuePair<string, string>("answers", "answers")
        }
    };

    private static readonly Dictionary<string, KeyValuePair<string, string>> PluralizeRuDict = new()
    {
        {
            "пользователь", new KeyValuePair<string, string>("пользователя", "пользователей")
        },
        {
            "бот", new KeyValuePair<string, string>("бота", "ботов")
        },
        {
            "роль", new KeyValuePair<string, string>("роли", "ролей")
        },
        {
            "канал", new KeyValuePair<string, string>("канала", "каналов")
        },
        {
            "минута", new KeyValuePair<string, string>("минуты", "минут")
        },
        {
            "ответ", new KeyValuePair<string, string>("ответа", "ответов")
        }
    };
}
