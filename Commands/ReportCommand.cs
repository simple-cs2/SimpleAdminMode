// Commands/ReportCommand.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !report &lt;target&gt; &lt;reason&gt; — Reports a player to admins via Telegram.
	/// </summary>
	private void OnReportCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;

		if(!_telegram.IsConfigured)
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Reports are not configured on this server!");
			return;
		}

		string targetArg = command.GetArg(1);
		string reason    = command.ArgCount > 2
			? string.Join(" ", Enumerable.Range(2, command.ArgCount - 2).Select(command.GetArg)).Trim()
			: "";

		if(string.IsNullOrEmpty(targetArg) || string.IsNullOrEmpty(reason))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!report <target> <reason>");
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

		_ = _telegram.SendReportAsync(
			player.PlayerName, player.SteamID,
			target.PlayerName, target.SteamID,
			reason,
			Server.MapName
		);

		player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Your report has been sent!");
	}
}