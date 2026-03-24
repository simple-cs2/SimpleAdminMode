// Commands/WarnCommand.cs
using System.Drawing;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
    /// <summary>
    /// !warn &lt;target&gt; [reason] — Issues a warning to a player.
    /// Automatically bans the player after reaching MaxWarns.
    /// </summary>
	private async void OnWarnCommand(CCSPlayerController? player, CommandInfo command)

	{
		if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/kick"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}warn{ChatColors.Default}'!");
			return;
		}

		string targetArg 	= command.GetArg(1);
		string reason 		= command.ArgCount > 2
			? string.Join(" ", Enumerable.Range(2, command.ArgCount - 2).Select(command.GetArg)).Trim()
			: "none";

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!warn <target> [reason]");
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

		var warn = new WarnEntry
        {
            PlayerSteamID       = target.SteamID,
            PlayerName          = target.PlayerName,
            AdminSteamID        = player.SteamID,
            AdminName           = player.PlayerName,
            Reason              = reason
        };
        await _database.Warns.AddAsync(warn);

        int warnCount = await _database.Warns.GetCountAsync(target.SteamID);
        int maxWarns = Config.MaxWarns;

        Server.NextFrame(() =>
        {
            if(!target.IsValid) return;

            SAMUtils.PrintActionToChat(player, targetArg, [target], "warned",
                $" {ChatColors.Grey}({reason}) {ChatColors.Default}[{ChatColors.Red}{warnCount}{ChatColors.Default}/{ChatColors.Red}{maxWarns}{ChatColors.Default}]");

            var message =
                $"<font color='#ff0000' size='24'><b>WARN {warnCount}/{maxWarns}</b></font><br><font color='#666666'>{'─'.ToString().PadRight(16, '─')}</font><br>" +
                $"<font color='#aaaaaa'>{reason}</font><br>" +
                $"<font color='#666666'>by </font><font color='#ffcc00'>{player.PlayerName}</font>";

            if(warnCount == maxWarns - 1)
                message += $"<br><font color='#666666'>{'─'.ToString().PadRight(16, '─')}</font><br><font color='#ff6600'><b>Next warn = ban</b></font>";

            if(_warnTimers.TryGetValue(target.SteamID, out var existingTimer))
            {
                existingTimer.Kill();
                _warnTimers.Remove(target.SteamID);
            }

            // Display player info about of warn
            var endTime = Server.CurrentTime + 10.0f;
            var timer = AddTimer(0.05f, () =>
            {
                if(!target.IsValid || Server.CurrentTime >= endTime)
                {
                    _warnTimers.Remove(target.SteamID);
                    return;
                }
                target.PrintToCenterHtml(message);
            }, TimerFlags.REPEAT);

            _warnTimers[target.SteamID] = timer;

            // Auto-ban if limit reached
            if(warnCount >= maxWarns)
            {
                var ban = new BanEntry
                {
                    PlayerSteamID       = target.SteamID,
                    PlayerName          = target.PlayerName,
                    AdminSteamID        = player.SteamID,
                    AdminName           = player.PlayerName,
                    Reason              = Config.WarnBanReason,
                    CreatedAt           = DateTime.Now,
                    ExpiredAt           = Config.WarnBanDuration == 0
                        ? new DateTime(9999, 12, 31, 23, 59, 59)
                        : DateTime.Now.AddMinutes(Config.WarnBanDuration)
                };

                _ = _database.Bans.AddAsync(ban);
                _ = _database.Warns.RemoveAllAsync(target.SteamID);

                Server.ExecuteCommand($"kickid {target.UserId} {Config.WarnBanReason}");
                SAMUtils.PrintActionToChat(null, target.PlayerName, [target], "banned",
                    $" {ChatColors.Default}for {ChatColors.Red}{SAMUtils.FormatDuration(Config.WarnBanDuration)} {ChatColors.Grey}({Config.WarnBanReason})");
            }
        });
	}

    /// <summary>
    /// !unwarn &lt;target&gt; — Removes the most recent warning from a player.
    /// </summary>
	private async void OnUnWarnCommand(CCSPlayerController? player, CommandInfo command)
	{
		if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/kick"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}unwarn{ChatColors.Default}'!");
			return;
		}

		string targetArg 	= command.GetArg(1);

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!unwarn <target>");
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

		int warnCount = await _database.Warns.GetCountAsync(target.SteamID);

        if(warnCount == 0)
        {
            Server.NextFrame(() => player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}{target.PlayerName} has no warns!"));
            return;
        }

        await _database.Warns.RemoveLastAsync(target.SteamID);

        Server.NextFrame(() => SAMUtils.PrintActionToChat(player, target.PlayerName, [target], "unwarned", $" {ChatColors.Grey}({warnCount - 1}/{Config.MaxWarns} warns remaining)"));
	}


    /// <summary>
    /// !warnlist &lt;target&gt; — Removes the most recent warning from a player.
    /// </summary>
    private async void OnWarnListCommand(CCSPlayerController? player, CommandInfo command)
    {
        if(player == null) return;

		if(!AdminManager.PlayerHasPermissions(player, "@css/kick"))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You don't have permission to use '{ChatColors.Red}warnlist{ChatColors.Default}'!");
			return;
		}

		string targetArg 	= command.GetArg(1);

		if(string.IsNullOrEmpty(targetArg))
		{
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Usage: {ChatColors.Grey}!warnlist <target>");
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

        var warns = await _database.Warns.GetAllAsync(target.SteamID);
        Server.NextFrame(() =>
        {
            if(warns.Count == 0)
            {
                Server.NextFrame(() => player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}{target.PlayerName} has no warns!"));
                return;
            }

            player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Gold}━━━ Warns: {ChatColors.Lime}{target.PlayerName} {ChatColors.Gold}({warns.Count}/{Config.MaxWarns}) ━━━");
            foreach(var warn in warns)
                player.PrintToChat($" {ChatColors.Grey} [{warn.CreatedAt:MM-dd HH:mm}] {ChatColors.Default}{warn.Reason} {ChatColors.Grey}by {warn.AdminName}");
        });
    }
}