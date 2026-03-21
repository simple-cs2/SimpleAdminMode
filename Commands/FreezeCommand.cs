// Commands/FreezeCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
    /// <summary>
    /// !freeze &lt;target&gt; — Freezes a player in place.
    /// !unfreeze &lt;target&gt; — Unfreezes a player.
    /// </summary>
	private void OnFreezeCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/slay"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}freeze{ChatColors.Default}'!");
			return;
		}

		string targetArg = command.GetArg(1);

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!freeze <target>");
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
            if(!AdminManager.CanPlayerTarget(player, target)) continue;
            if(target.PlayerPawn.Value == null) continue;

			SetMoveType(target.PlayerPawn.Value, MoveType_t.MOVETYPE_OBSOLETE);
        }

		SAMUtils.PrintActionToChat(player, targetArg, targets, "frozen");
	}

    private void OnUnFreezeCommand(CCSPlayerController? player, CommandInfo command)
    {
        if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/slay"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}unfreeze{ChatColors.Default}'!");
			return;
		}

		string targetArg 	= command.GetArg(1);

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!unfreeze <target>");
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
            if(!AdminManager.CanPlayerTarget(player, target)) continue;
            if(target.PlayerPawn.Value == null) continue;

			SetMoveType(target.PlayerPawn.Value, MoveType_t.MOVETYPE_WALK);
        }

		SAMUtils.PrintActionToChat(player, targetArg, targets, "unfrozen");
    }

	private void SetMoveType(CCSPlayerPawn pawn, MoveType_t moveType)
	{
		pawn.MoveType = moveType;
		Utilities.SetStateChanged(pawn, "CBaseEntity", "m_MoveType");
		Schema.GetRef<MoveType_t>(pawn.Handle, "CBaseEntity", "m_nActualMoveType") = moveType;
	}
}