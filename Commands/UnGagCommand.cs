// Commands/UnGagCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !ungag &lt;steamid64&gt; — Unblocks text chat for a player.
	/// </summary>
	private async void OnUnGagCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;
	
		if(!AdminManager.PlayerHasPermissions(player, "@css/chat"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}ungag{ChatColors.Default}'!");
			return;
		}

		string targetArg = command.GetArg(1);
		
		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!ungag <steamid64>");
			return;
		}

		if(!ulong.TryParse(targetArg, out ulong steamId))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Invalid SteamID64 format!");
			return;
		}

		var gag = await _database.Gags.GetActiveAsync(steamId); 

		Server.NextFrame(() =>
		{
			if(gag == null)
			{
				player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Can't find a player to target ({ChatColors.Lime}{targetArg}{ChatColors.Default})!");
				return;
			}

			_ = _database.Gags.RemoveAsync(steamId);
			_gagsCache.Remove(steamId);

			SAMUtils.PrintActionToChat(player, gag.PlayerName, null, "ungagged");
		});
	}

}