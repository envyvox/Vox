using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Vox.Data.Enums;
using Vox.Services.Discord.Client;
using Vox.Services.Discord.Client.Extensions;

namespace Vox.Services.Discord.Interactions.Components.RolePicker;

public class RolePickerDropdown : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("role-picker-dropdown")]
    public async Task Execute(string[] selectedValues)
    {
        await DeferAsync(true);

        var originalResponse = await GetOriginalResponseAsync();
        var actionRow = (ActionRowComponent) originalResponse.Components.First();
        var selectMenu = (SelectMenuComponent) actionRow.Components.First();
        var dropdownRoles = selectMenu.Options
            .Select(option => Context.Guild.GetRole(ulong.Parse(option.Value)));

        var selectedRoles = selectedValues
            .Select(roleIdString => Context.Guild.GetRole(ulong.Parse(roleIdString)))
            .Where(role => role is not null).ToList();

        var socketGuildUser = Context.Guild.GetUser(Context.User.Id);
        var userRoles = socketGuildUser.Roles
            .Where(userRole => dropdownRoles
                .Select(dropdownRole => dropdownRole.Id)
                .Contains(userRole.Id))
            .ToList();

        var rolesToRemove = userRoles
            .Where(userRole => selectedRoles
                .Select(selectedRole => selectedRole.Id)
                .Contains(userRole.Id) is false)
            .ToList();

        var rolesToAdd = selectedRoles
            .Where(selectedRole => userRoles
                .All(userRole => userRole.Id != selectedRole.Id))
            .ToList();

        var addedRoles = rolesToAdd.Aggregate(string.Empty, (s, v) => s + $"{v.Mention}, ");
        var removedRoles = rolesToRemove.Aggregate(string.Empty, (s, v) => s + $"{v.Mention}, ");

        await socketGuildUser.RemoveRolesAsync(rolesToRemove);
        await socketGuildUser.AddRolesAsync(rolesToAdd);

        await FollowupAsync(
            (addedRoles.Length > 0
                ? Response.SuccessfullyReceivedRoles.Parse(Context.Guild.PreferredLocale,
                    addedRoles.RemoveFromEnd(2))
                : "") +
            (removedRoles.Length > 0
                ? Response.SuccessfullyRemovedRoles.Parse(Context.Guild.PreferredLocale,
                    removedRoles.RemoveFromEnd(2))
                : ""),
            ephemeral: true);
    }
}