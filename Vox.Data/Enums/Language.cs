namespace Vox.Data.Enums;

public enum Language : byte
{
    English = 1,
    Russian = 2
}

public static class LanguageHelper
{
    public static Language GetLanguageFromPreferredLocale(this string preferredLocale)
    {
        return preferredLocale switch
        {
            "ru" => Language.Russian,
            _ => Language.English
        };
    }
}