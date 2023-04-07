using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Interactions;
using Vox.Data.Enums;

namespace Vox.Services.Discord.Interactions.Components.RolePicker;

public class RolePickerButton : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("role-picker-button:*")]
    public async Task Execute(string roleId)
    {
        await DeferAsync(true);

        var role = Context.Guild.GetRole(ulong.Parse(roleId));

        if (role is null)
        {
            throw new Exception(Response.RoleNotFoundInGuild.Parse(Context.Guild.PreferredLocale,
                roleId, Context.Guild.Id));
        }

        var socketGuildUser = Context.Guild.GetUser(Context.User.Id);

        if (socketGuildUser.Roles.Any(x => x.Id == role.Id))
        {
            await socketGuildUser.RemoveRoleAsync(role);

            await FollowupAsync(
                Response.SuccessfullyRemovedRole.Parse(Context.Guild.PreferredLocale, role.Mention),
                ephemeral: true);
        }
        else
        {
            await socketGuildUser.AddRoleAsync(role);

            await FollowupAsync(
                Response.SuccessfullyReceivedRole.Parse(Context.Guild.PreferredLocale, role.Mention),
                ephemeral: true);
        }
    }
}