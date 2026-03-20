// AdminMenu.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using MenuManager;

namespace SimpleAdminMode;

public class AdminMenu
{
	private readonly SimpleAdminMode _plugin;
	private IMenuApi? _api;
	private readonly PluginCapability<IMenuApi?> _pluginCapability = new("menu:nfcore");

	// Players waiting to input custom duration in chat
	// Key: SteamID, Value: (target, action)
	private Dictionary<ulong, (CCSPlayerController target, string action)> _pendingDuration = new();

	// Players waiting to input custom reason in chat
	// Key: SteamID, Value: (target, action, minutes)
	private Dictionary<ulong, (CCSPlayerController target, string action, int minutes)> _pendingReason = new();

	public bool IsInitialized => _api != null;

	public AdminMenu(SimpleAdminMode plugin)
	{
		_plugin = plugin;
	}

	public void Initialize()
	{
		try
		{
			_api = _pluginCapability.Get();
			Console.WriteLine(_api != null
				? "[SAM] ✅ MenuManager loaded!"
				: "[SAM] ❌ MenuManager is null!");
		}
		catch(Exception ex)
		{
			Console.WriteLine($"[SAM] ❌ MenuManager error: {ex.Message}");
		}
	}

	// Menus --!>>
	public void OpenMainMenu(CCSPlayerController? player)
	{
		if(player == null || _api == null) return;

		var menu = _api.GetMenu("[SAM] Main Menu");

		if(AdminManager.PlayerHasPermissions(player, "@css/slay"))
			menu.AddMenuOption("Slay",		(p, _) => OpenPlayerMenu(p, "slay"));

		if(AdminManager.PlayerHasPermissions(player, "@css/kick"))
			menu.AddMenuOption("Kick",		(p, _) => OpenPlayerMenu(p, "kick"));

		if(AdminManager.PlayerHasPermissions(player, "@css/ban"))
			menu.AddMenuOption("Ban",		(p, _) => OpenPlayerMenu(p, "ban"));

		if(AdminManager.PlayerHasPermissions(player, "@css/chat"))
		{
			menu.AddMenuOption("Gag",		(p, _) => OpenPlayerMenu(p, "gag"));
			menu.AddMenuOption("Mute",		(p, _) => OpenPlayerMenu(p, "mute"));
			menu.AddMenuOption("Silence",	(p, _) => OpenPlayerMenu(p, "silence"));
		}

		if(AdminManager.PlayerHasPermissions(player, "@css/rename"))
			menu.AddMenuOption("Rename",	(p, _) => OpenPlayerMenu(p, "rename"));
		
		menu.Open(player);
	}

	private void OpenPlayerMenu(CCSPlayerController admin, string action)
	{
		if(_api == null) return;

		var menu = _api.GetMenu($"[SAM] {char.ToUpper(action[0]) + action[1..]} - Select Player");

		var players = Utilities.GetPlayers()
			.Where(p => p.IsValid /*&& !p.IsBot*/)
			.OrderBy(p => p.PlayerName);

		foreach(var target in players)
		{
			bool disabled = !AdminManager.CanPlayerTarget(admin, target);

			// Additional validation depending on the action
			switch(action)
			{
				case "slay":
					disabled = disabled || !target.PawnIsAlive;
					break;
			}

			menu.AddMenuOption(target.PlayerName, (p, _) =>
			{
				if(!target.IsValid)
				{
					admin.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Player is no longer on the server!");
					return;
				}

				if(action is "slay")
					HandleAction(admin, target, action);
				else if(action is "kick")
					OpenReasonMenu(p, target, action, 0);
				else if(action is "rename")
					OpenRenameInput(p, target);
				else
					OpenDurationMenu(p, target, action);
			}, disabled);
		}

		menu.Open(admin);
	}

	private void OpenDurationMenu(CCSPlayerController admin, CCSPlayerController target, string action)
	{
		if(_api == null) return;

		var menu = _api.GetMenu($"[SAM] {char.ToUpper(action[0]) + action[1..]} - Select Duration");

		menu.AddMenuOption("Custom duration...", (p, _) =>
		{
			_pendingDuration[p.SteamID] = (target, action);
			p.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Type your duration in chat. {ChatColors.Grey}(type {ChatColors.White}!cancel {ChatColors.Grey}to cancel)");
		});

		// Get predefined durations from config
		var durations = GetDuration(action);

		foreach(var (label, minutes) in durations)
			menu.AddMenuOption(label, (p, _) => OpenReasonMenu(p, target, action, minutes));
		
		menu.Open(admin);
	}

	private void OpenReasonMenu(CCSPlayerController admin, CCSPlayerController target, string action, int minutes)
	{
		if(_api == null) return;

		var menu = _api.GetMenu($"[SAM] {char.ToUpper(action[0]) + action[1..]} - Select Reason");
		
		menu.AddMenuOption("Custom reason...", (p, _) =>
		{
			_pendingReason[p.SteamID] = (target,action, minutes);
			p.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Type your reason in chat. {ChatColors.Grey}(type {ChatColors.White}!cancel {ChatColors.Grey}to cancel)");
		});

		// Get predefined reasons from config
		var reasons = GetReasons(action);

		foreach(var reason in reasons)
			menu.AddMenuOption(reason, (p, _) => HandleAction(p, target, action, minutes, reason));

		menu.Open(admin);
	}

	private void OpenRenameInput(CCSPlayerController admin, CCSPlayerController target)
	{
		_pendingReason[admin.SteamID] = (target, "rename", 0);
    	admin.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Type new name for {ChatColors.Lime}{target.PlayerName}{ChatColors.Default} in chat. {ChatColors.Grey}(type {ChatColors.White}!cancel {ChatColors.Grey}to cancel)");
	}

	private void HandleAction(CCSPlayerController admin, CCSPlayerController target, string action, int minutes = 0, string reason = "none")
	{
		if(!target.IsValid)
		{
            admin.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Player is no longer on the server!");
            return;
        }

		switch(action)
		{
			case "slay":
				target.PlayerPawn.Value?.CommitSuicide(false, true);
                SAMUtils.PrintActionToChat(admin, target.PlayerName, [target], "slayed");
                break;
				
			case "kick":
				Server.ExecuteCommand($"kickid {target.UserId} {reason}");
				SAMUtils.PrintActionToChat(admin, target.PlayerName, [target], "kicked", $" {ChatColors.Grey}({reason})");
                break;
				
			case "ban":
				var ban = new BanEntry
				{
					PlayerSteamID	= target.SteamID,
					PlayerName		= target.PlayerName,
					AdminSteamID	= admin.SteamID,
					AdminName		= admin.PlayerName,
					Reason			= reason,
					CreatedAt		= DateTime.Now,
					ExpiredAt		= minutes == 0
						? new DateTime(9999, 12, 31, 23, 59, 59)
						: DateTime.Now.AddMinutes(minutes)	
				};
				_ = _plugin._database.Bans.AddAsync(ban);
				Server.ExecuteCommand($"kickid {target.UserId} {reason}");
				SAMUtils.PrintActionToChat(admin, target.PlayerName, [target], "banned",
                    $" {ChatColors.Default}for {ChatColors.Red}{SAMUtils.FormatDuration(minutes)} {ChatColors.Grey}({reason})");
                break;
				
			case "gag":
				var gag = new GagEntry
				{
					PlayerSteamID	= target.SteamID,
					PlayerName		= target.PlayerName,
					AdminSteamID	= admin.SteamID,
					AdminName		= admin.PlayerName,
					Reason			= reason,
					CreatedAt		= DateTime.Now,
					ExpiredAt		= minutes == 0
						? new DateTime(9999, 12, 31, 23, 59, 59)
						: DateTime.Now.AddMinutes(minutes)
				};
				_ = _plugin._database.Gags.AddAsync(gag);
				_plugin._gagsCache[target.SteamID] = gag;
                SAMUtils.PrintActionToChat(admin, target.PlayerName, [target], "gagged",
					$" {ChatColors.Default}for {ChatColors.Red}{SAMUtils.FormatDuration(minutes)} {ChatColors.Grey}({reason})");
				break;
				
			case "mute":
				var mute = new MuteEntry
				{
					PlayerSteamID	= target.SteamID,
					PlayerName		= target.PlayerName,
					AdminSteamID	= admin.SteamID,
					AdminName		= admin.PlayerName,
					Reason			= reason,
					CreatedAt		= DateTime.Now,
					ExpiredAt		= minutes == 0
						? new DateTime(9999, 12, 31, 23, 59, 59)
						: DateTime.Now.AddMinutes(minutes)
				};
				_ = _plugin._database.Mutes.AddAsync(mute);
				_plugin._mutesCache[target.SteamID] = mute;
                SAMUtils.PrintActionToChat(admin, target.PlayerName, [target], "muted",
					$" {ChatColors.Default}for {ChatColors.Red}{SAMUtils.FormatDuration(minutes)} {ChatColors.Grey}({reason})");
				break;

			case "silence":
				// Gag + Mute combined — reuse logic:
				HandleAction(admin, target, "gag",  minutes, reason);
				HandleAction(admin, target, "mute", minutes, reason);
				return; // skip CloseMenu — already called in recursive calls
				
			case "rename":
				string oldName = target.PlayerName;
				SAMUtils.PrintActionToChat(admin, oldName, [target], "renamed",
					$" {ChatColors.Default}to {ChatColors.Blue}{reason}");

				// We change the player's nickname after sending a message in the chat about it, for the sake of correctness
				target.PlayerName = reason;
				Utilities.SetStateChanged(target, "CBasePlayerController", "m_iszPlayerName");
				break;
		}

		_api?.CloseMenu(admin);
	}


	// Helpers --!>>
	/// <summary>
    /// Called from OnPlayerChat in SimpleAdminMode.cs to handle custom reason input.
    /// Returns true if message was consumed.
    /// </summary>
	public bool HandleChatInput(CCSPlayerController player, string message)
	{
		var steamId = player.SteamID;

		if(_pendingDuration.TryGetValue(steamId, out var durationPending))
		{
			_pendingDuration.Remove(steamId);

			if(message.Equals("!cancel", StringComparison.OrdinalIgnoreCase))
			{
				player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Cancelled.");
				return true;
			}

			int minutes = SAMUtils.ParseDuration(message);
			if(minutes == 0 && message != "0")
			{
				if(!int.TryParse(message, out minutes))
				{
					player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Invalid duration! {ChatColors.Grey}(e.g. {ChatColors.White}30{ChatColors.Grey}, {ChatColors.White}1h30m{ChatColors.Grey}, {ChatColors.White}1d{ChatColors.Grey}, {ChatColors.White}0 {ChatColors.Grey}= permanent)");
					return true;
				}
			}

			OpenReasonMenu(player, durationPending.target, durationPending.action, minutes);
			return true;

		}

		if(_pendingReason.TryGetValue(steamId, out var reasonPending))
		{
			_pendingReason.Remove(steamId);

			if(message.Equals("!cancel", StringComparison.OrdinalIgnoreCase))
			{
				player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}Cancelled.");
				return true;
			}

			HandleAction(player, reasonPending.target, reasonPending.action, reasonPending.minutes, message);
			return true;

		}

        return false;
	}

	private List<string> GetReasons(string action) => action switch
	{
		"slay"		=> _plugin.Config.SlayReasons,
		"kick"		=> _plugin.Config.KickReasons,
		"ban"		=> _plugin.Config.BanReasons,
		"gag"		=> _plugin.Config.GagReasons,
		"mute"		=> _plugin.Config.MuteReasons,
		"silence"	=> _plugin.Config.SilenceReason,
		_			=> new List<string> { "none" },
	};

	private Dictionary<string, int> GetDuration(string action) => action switch
	{
		"ban"		=> _plugin.Config.BanDuration,	
		"gag"		=> _plugin.Config.GagDuration,	
		"mute"		=> _plugin.Config.MuteDuration,		
		"silence"	=> _plugin.Config.SilenceDuration,	
		_			=> new Dictionary<string, int> { { "5 minutes",  30 } }
	};
}