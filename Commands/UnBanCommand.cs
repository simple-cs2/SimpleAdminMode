// Commands/UnBanCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !unban &lt;steamid64&gt; — Unbans a player.
	/// </summary>
	private async void OnUnBanCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;
	
		if(!AdminManager.PlayerHasPermissions(player, "@css/ban"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}unban{ChatColors.Default}'!");
			return;
		}

		string targetArg = command.GetArg(1);

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!unban <steamid64>");
			return;
		}

		if(!ulong.TryParse(targetArg, out ulong steamId))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Invalid SteamID64 format!");
			return;
		}

		var ban = await _database.Bans.GetActiveAsync(steamId);

		Server.NextFrame(() =>
		{
			if(ban == null)
			{
				player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Can't find a player to target ({ChatColors.Lime}{targetArg}{ChatColors.Default})!");
				return;
			}

			_ = _database.Bans.RemoveAsync(ban.PlayerSteamID);
			SAMUtils.PrintActionToChat(player, ban.PlayerName, null, "unbanned");
		});
	}

}