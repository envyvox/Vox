﻿using System.Globalization;
using Discord;

namespace Vox.Services.Discord.Embed;

public static class EmbedBuilderExtensions
{
    public const string DefaultEmbedColor = "36393F";
    public const string GoldEmbedColor = "ffe247";

    public static EmbedBuilder AddEmptyField(this EmbedBuilder builder, bool inline)
    {
        return builder.AddField("⠀", "⠀", inline);
    }

    public static EmbedBuilder WithDefaultColor(this EmbedBuilder builder)
    {
        return builder.WithColor(new Color(uint.Parse(DefaultEmbedColor, NumberStyles.HexNumber)));
    }

    public static EmbedBuilder WithGoldColor(this EmbedBuilder builder)
    {
        return builder.WithColor(new Color(uint.Parse(GoldEmbedColor, NumberStyles.HexNumber)));
    }

    public static EmbedBuilder WithUserColor(this EmbedBuilder builder, string commandColor)
    {
        return builder.WithColor(new Color(uint.Parse(commandColor, NumberStyles.HexNumber)));
    }
}
