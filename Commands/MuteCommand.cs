// commands/MuteCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !mute &lt;target&gt; &lt;duration&gt; [reason] - Blocks voice chat for a player.
	/// </summary>
	private void OnMuteCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/chat"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}mute{ChatColors.Default}'!");
			return;
		}

		string targetArg    = command.GetArg(1);
		string duration     = command.GetArg(2);
		string reason       = command.ArgCount > 2
			? string.Join(" ", Enumerable.Range(3, command.ArgCount - 3).Select(command.GetArg)).Trim()
			: "none";

		if(string.IsNullOrEmpty(targetArg) || string.IsNullOrEmpty(duration))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!mute <target> <duration> [reason]");
			return;
		}

		int minutes = SAMUtils.ParseDuration(duration);
		if(minutes == 0 && duration != "0")
		{
			if(!int.TryParse(duration, out minutes))
			{
				player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Invalid duration! {ChatColors.Grey}(e.g. {ChatColors.White}30{ChatColors.Grey}, {ChatColors.White}1h30m{ChatColors.Grey}, {ChatColors.White}1d{ChatColors.Grey}, {ChatColors.White}0 {ChatColors.Grey}= permanent)");
				return;
			}
		}

		var targets = SAMUtils.GetTargets(player, targetArg);

		if(targets.Count == 0)
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Can't find a player to target ({ChatColors.Lime}{targetArg}{ChatColors.Default})!");
			return;
		}

		if(targets.Count > 1)
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You can't target multiple players using this command!");
			return;
		}

		var target = targets.First();

		if(!AdminManager.CanPlayerTarget(player, target))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You can't target {ChatColors.Red}{target.PlayerName}{ChatColors.Default}!");
			return;
		}

		var mute = new MuteEntry
		{
			PlayerSteamID   = target.SteamID,
			PlayerName      = target.PlayerName,
			AdminSteamID    = player.SteamID,
			AdminName       = player.PlayerName,
			Reason          = reason,
			CreatedAt       = DateTime.Now,
			ExpiredAt       = minutes == 0
				? new DateTime(9999, 12, 31, 23, 59, 59)
				: DateTime.Now.AddMinutes(minutes)
		};

		target.VoiceFlags = VoiceFlags.Muted;
		_ = _database.Mutes.AddAsync(mute);
		_mutesCache[target.SteamID] = mute;

		SAMUtils.PrintActionToChat(player, targetArg, [target], "muted", $" {ChatColors.Default}for {ChatColors.Red}{SAMUtils.FormatDuration(minutes)} {ChatColors.Grey}({reason})");
	}
}