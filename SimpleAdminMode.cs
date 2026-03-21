// SimpleAdminMode.cs
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

public partial class SimpleAdminMode : BasePlugin, IPluginConfig<PluginConfig>
{
	public override string ModuleName => "SimpleAdminMode";
	public override string ModuleVersion => "0.1.0";
	public override string ModuleAuthor => "t.me/kotyarakryt";
	public override string ModuleDescription => "A lightweight and powerful admin system for CS2 servers built on CounterStrikeSharp.";

	public PluginConfig Config { get; set; } = new();

	internal Database _database = null!;
	private TelegramService _telegram = null!;
	private AdminMenu _adminMenu = null!;

	// In-memory caches for active gags and mutes.
	// Populated on plugin load and kept in sync with the database.
	internal Dictionary<ulong, GagEntry> _gagsCache = new();
	internal Dictionary<ulong, MuteEntry> _mutesCache = new();

	public override void Load(bool hotReload)
	{
		// Database
		string connString = $"Server={Config.Host};Port={Config.Port};Database={Config.Database};User={Config.Username};Password={Config.Password};";
		_database = new Database(connString);
		_ = _database.InitializeAsync();

		// Cache
		_ = LoadGagCacheAsync();
		_ = LoadMuteCacheAsync();

		// AdminMenu
		_adminMenu = new AdminMenu(this);

		// Events
		RegisterEventHandler<EventPlayerConnectFull>((@event, info) =>
		{
			_ = OnPlayerConnect(@event, info);
			return HookResult.Continue;
		});
		RegisterEventHandler<EventPlayerChat>(OnPlayerChat);
		RegisterListener<Listeners.OnClientVoice>(OnPlayerVoice);

		// Commands
		AddCommand("css_admin",		"Open admin menu",				OnAdminMenuCommand);
		AddCommand("css_slay",		"Kill a player",				OnSlayCommand);
		AddCommand("css_kick",		"Kick a player",				OnKickCommand);
		AddCommand("css_ban",		"Ban a player",					OnBanCommand);
		AddCommand("css_unban",		"Unban a player",				OnUnBanCommand);
		AddCommand("css_gag",		"Block text chat",				OnGagCommand);
		AddCommand("css_ungag",		"Unblock text chat",			OnUnGagCommand);
		AddCommand("css_mute",		"Block voice chat",				OnMuteCommand);
		AddCommand("css_unmute",	"Unblock voice chat",			OnUnMuteCommand);
		AddCommand("css_silence",	"Block all communication",		OnSilenceCommand);
		AddCommand("css_unsilence",	"Unblock all communication",	OnUnSilenceCommand);
		AddCommand("css_rename",	"Rename a player",				OnRenameCommand);
		AddCommand("css_report",	"Report a player to Telegram",	OnReportCommand);
		AddCommand("css_freeze",	"Freeze a player",				OnFreezeCommand);
		AddCommand("css_unfreeze",	"Unfreeze a player",			OnUnFreezeCommand);
	}

	public override void OnAllPluginsLoaded(bool hotReload)
	{
		_adminMenu.Initialize();
	}

	public void OnConfigParsed(PluginConfig config)
	{
		Config = config;
		_telegram = new TelegramService(config.TelegramBotToken, config.TelegramChatId);
	}

	private async Task OnPlayerConnect(EventPlayerConnectFull @event, GameEventInfo info)
	{
		var player = @event.Userid;
		if(player == null || !player.IsValid) return;

		var ban  = await _database.Bans.GetActiveAsync(player.SteamID);
		var gag  = await _database.Gags.GetActiveAsync(player.SteamID);
		var mute = await _database.Mutes.GetActiveAsync(player.SteamID);

		Server.NextFrame(() =>
		{
			if(!player.IsValid) return;

			if(ban != null)
				Server.ExecuteCommand($"kickid {player.UserId} You are banned! Reason: {ban.Reason}");

			if(gag  != null) _gagsCache[player.SteamID]  = gag;
			if(mute != null) _mutesCache[player.SteamID] = mute;
		});
	}

	private HookResult OnPlayerChat(EventPlayerChat @event, GameEventInfo info)
	{
		var player = Utilities.GetPlayerFromUserid(@event.Userid);
		if(player == null) return HookResult.Continue;

		// Handle custom reason input for admin menu
		if(_adminMenu.IsInitialized && _adminMenu.HandleChatInput(player, @event.Text))
			return HookResult.Handled;

		if(_gagsCache.TryGetValue(player.SteamID, out var gag) && gag.ExpiredAt > DateTime.Now)
		{
			bool isPermanent = gag.ExpiredAt == new DateTime(9999, 12, 31, 23, 59, 59);
			string duration  = isPermanent ? "Indefinitely" : $"until {gag.ExpiredAt:yyyy-MM-dd HH:mm}";
			player.PrintToChat($" {ChatColors.Red}[SAM] {ChatColors.Default}You are gagged. {ChatColors.Grey}({duration})");
			return HookResult.Handled;
		}

		return HookResult.Continue;
	}

	private void OnPlayerVoice(int clientSlot)
	{
		var player = Utilities.GetPlayerFromSlot(clientSlot);
		if(player == null) return;

		if(_mutesCache.TryGetValue(player.SteamID, out var mute) && mute.ExpiredAt > DateTime.Now)
			player.VoiceFlags = VoiceFlags.Muted;
	}

	private async Task LoadGagCacheAsync()
	{
		var gags   = await _database.Gags.GetAllActiveAsync();
		_gagsCache = gags.ToDictionary(g => g.PlayerSteamID);

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"[SAM] ✓ Loaded {_gagsCache.Count} active gags into cache.");
		Console.ResetColor();
	}

	private async Task LoadMuteCacheAsync()
	{
		var mutes   = await _database.Mutes.GetAllActiveAsync();
		_mutesCache = mutes.ToDictionary(m => m.PlayerSteamID);

		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"[SAM] ✓ Loaded {_mutesCache.Count} active mutes into cache.");
		Console.ResetColor();
	}
}