// Commands/KickCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !kick &lt;target&gt; [reason] - Kicks a player from the server.
	/// </summary>
	private void OnKickCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/kick"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}kick{ChatColors.Default}'!");
			return;
		}

		string targetArg 	= command.GetArg(1);
		string reason 		= command.ArgCount > 2
			? string.Join(" ", Enumerable.Range(2, command.ArgCount - 2).Select(command.GetArg)).Trim()
			: "none";

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!kick <target> [reason]");
			return;
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

		Server.ExecuteCommand($"kickid {target.UserId} {reason}");
		SAMUtils.PrintActionToChat(player, targetArg, [target], "kicked", $" {ChatColors.Grey}({reason})");
	}
}