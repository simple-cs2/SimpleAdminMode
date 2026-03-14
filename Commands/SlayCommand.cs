// Commands/SlayCommand.cs
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !slay &lt;target&gt; - Kills a player or all players.
	/// </summary>
	private void OnSlayCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/slay"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}slay{ChatColors.Default}'!");
			return;
		}

		string targetArg = command.GetArg(1);

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!slay <target>");
			return;
		}

		var targets = SAMUtils.GetTargets(player, targetArg);

		if(targets.Count == 0)
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Can't find a player to target ({ChatColors.Lime}{targetArg}{ChatColors.Default})!");
			return;
		}

		foreach(var target in targets)
		{
			if(!AdminManager.CanPlayerTarget(player, target) || !target.PawnIsAlive) continue;
			target.PlayerPawn.Value?.CommitSuicide(false, true);
		}

		SAMUtils.PrintActionToChat(player, targetArg, targets, "slayed");
	}

}