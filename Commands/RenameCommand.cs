// Commands/RenameCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !rename &lt;target&gt; &lt;name&gt; — Renames a player.
	/// Supports multi-word names: !rename player Cool New Name
	/// </summary>
	private void OnRenameCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;
	
		if(!AdminManager.PlayerHasPermissions(player, "@css/rename"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}rename{ChatColors.Default}'!");
			return;
		}

		string targetArg = command.GetArg(1);
		string newName   = command.ArgCount > 2
			? string.Join(" ", Enumerable.Range(2, command.ArgCount - 2).Select(command.GetArg)).Trim()
			: "";

		if(string.IsNullOrEmpty(targetArg) || string.IsNullOrEmpty(newName))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!rename <target> <new name>");
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

		string oldName = target.PlayerName;
		target.PlayerName = newName;
		Utilities.SetStateChanged(target, "CBasePlayerController", "m_iszPlayerName");
		
		SAMUtils.PrintActionToChat(player, oldName, null, "renamed", $" {ChatColors.Default} to {ChatColors.Blue}{newName}");
	}
}