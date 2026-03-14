// Commands/UnMuteCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !unmute &lt;steamid&gt; — Unblocks voice chat for a player.
	/// </summary>
	private async void OnUnMuteCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/chat"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}unmute{ChatColors.Default}'!");
			return;
		}

		string targetArg = command.GetArg(1);
		
		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!unmute <steamid64>");
			return;
		}

		if(!ulong.TryParse(targetArg, out ulong steamId))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Invalid SteamID64 format!");
			return;
		}

		var mute = await _database.Mutes.GetActiveAsync(steamId); 

		Server.NextFrame(() =>
		{
			if(mute == null)
			{
				player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Can't find a player to target ({ChatColors.Lime}{targetArg}{ChatColors.Default})!");
				return;
			}

			// Restore voice if player is currently on the server:
			var target = Utilities.GetPlayers().FirstOrDefault(m => m.SteamID == steamId);
			if(target != null) target.VoiceFlags = VoiceFlags.Normal;

			_ = _database.Mutes.RemoveAsync(steamId);
			_mutesCache.Remove(steamId);

			SAMUtils.PrintActionToChat(player, mute.PlayerName, null, "unmuted");
		});
	}

}