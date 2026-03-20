// Commands/AdminMenuCommand.cs
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
    /// <summary>
    /// !admin — Opens the admin menu.
    /// </summary>
    private void OnAdminMenuCommand(CCSPlayerController? player, CommandInfo command)
    {
        if(player == null) return;
        if(!AdminManager.PlayerHasPermissions(player, "@css/generic"))
        {
            player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}admin{ChatColors.Default}'!");
            return;
        }

        _adminMenu.OpenMainMenu(player);
    }
}