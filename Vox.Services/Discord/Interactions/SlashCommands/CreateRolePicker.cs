using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Client.Extensions;
using Vox.Services.Discord.Embed;
using static Vox.Services.Discord.Client.Extensions.ExceptionExtensions;
using ComponentType = Vox.Data.Enums.ComponentType;

namespace Vox.Services.Discord.Interactions.SlashCommands;

[RequireUserPermission(GuildPermission.Administrator)]
public class CreateRolePicker : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("create-role-picker", "Creates a role picker to select the specified roles (max 25)")]
    public async Task Execute(
        [Summary("type", "Role picker type")] ComponentType componentType,
        [Summary("roles", "List of roles separated by spaces (use @mention)")]
        string input,
        [Summary("limit", "Number of roles that can be selected (only for the dropdown list)")]
        int roleLimit = 0)
    {
        await DeferAsync(true);

        var roles = input
            .Split(' ')
            .Where(x => x != "") // just check there is no empty items in array
            .Select(roleString => Context.Guild.GetRole(ulong.Parse(Regex.Replace(roleString, @"[^\d]", ""))))
            .Where(role => role is not null)
            .ToList();

        if (roleLimit > roles.Count) roleLimit = roles.Count;

        if (roles.Count is < 1 or > 25)
        {
            throw new ExceptionExtensions.ExpectedException(Response.WrongRolesCount.Parse(Context.Guild.PreferredLocale));
        }

        var components = new ComponentBuilder();

        switch (componentType)
        {
            case ComponentType.Buttons:
            {
                foreach (var socketRole in roles)
                {
                    components.WithButton(
                        socketRole.Name,
                        $"role-picker-button:{socketRole.Id}");
                }

                break;
            }
            case ComponentType.Dropdown:
            {
                var selectMenu = new SelectMenuBuilder()
                    .WithCustomId("role-picker-dropdown")
                    .WithPlaceholder(Response.SelectRolesFromDropdown.Parse(Context.Guild.PreferredLocale))
                    .WithMinValues(0)
                    .WithMaxValues(roleLimit is not 0 ? roleLimit : roles.Count);

                foreach (var socketRole in roles)
                {
                    selectMenu.AddOption(
                        socketRole.Name,
                        socketRole.Id.ToString());
                }

                components.WithSelectMenu(selectMenu);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(componentType), componentType, null);
        }

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithAuthor(
                Response.CreateRolePicker.Parse(Context.Guild.PreferredLocale),
                Context.User.GetAvatarUrl())
            .WithDescription(Response.SuccessfullyCreatedRolePicker.Parse(Context.Guild.PreferredLocale,
                Context.User.Mention));

        await Context.Channel.SendMessageAsync(StringExtensions.EmptyChar, components: components.Build());
        await FollowupAsync(embed: embed.Build());
    }
}
