// AdminMenu.cs
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Utils;
using MenuManager;

namespace SimpleAdminMode;

// TODO: Admin menu is currently in development.
// Planned features:
//   - Player selection with immunity support
//   - Action submenu (slay, kick, ban, mute, gag, silence)
//   - Duration picker for bans/mutes
//   - Reason picker per action type
//   - Back navigation between submenus
public class AdminMenu
{
	private IMenuApi? _api;
	private readonly PluginCapability<IMenuApi?> _pluginCapability = new("menu:nfcore");
	public bool IsInitialized => _api != null;

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

	// TODO: Open main admin menu with available actions based on player permissions
	public void OpenMainMenu(CCSPlayerController? player)
	{
		if(player == null) return;

		// Not implemented yet
		player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Grey}Admin menu is currently under development.");
	}

	// TODO: Open player selection submenu for a given action
	private void OpenPlayerMenu(CCSPlayerController admin, string action)
	{
		// Filter players based on action:
		// - slay: only alive players
		// - kick/ban/mute/gag: all connected players
		// - respect AdminManager.CanPlayerTarget() immunity
	}

	// TODO: Open duration picker (30m, 1h, 3h, 1d, 1w, 1mo, permanent)
	private void OpenDurationMenu(CCSPlayerController admin, CCSPlayerController target, string action)
	{
	}

	// TODO: Open reason picker based on action type
	private void OpenReasonMenu(CCSPlayerController admin, CCSPlayerController target, string action, int minutes)
	{
		// kick  → AFK, Texture, BodyBlock, Interference
		// ban   → Cheating, Griefing, BanEvasion, Toxic
		// mute  → Toxic, Spam, Screaming
		// gag   → Toxic, Spam, Advertising
	}

	// TODO: Execute the action after player/duration/reason are selected
	private void HandleAction(CCSPlayerController admin, CCSPlayerController target, string action, int minutes = 0, string reason = "none")
	{
	}
}