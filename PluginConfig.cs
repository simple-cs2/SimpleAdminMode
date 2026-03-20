// PluginConfig.cs
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace SimpleAdminMode;

/// <summary>
/// Plugin configuration — auto-generated at:
/// configs/plugins/SimpleAdminMode/SimpleAdminMode.json
/// </summary>
public class PluginConfig : BasePluginConfig
{
	[JsonPropertyName("Host")]
	public string Host { get; set; } = "127.0.0.1";
	[JsonPropertyName("Port")]
	public int Port { get; set; } = 3306;
	
	[JsonPropertyName("Database")]
	public string Database { get; set; } = "sam";
	
	[JsonPropertyName("Username")]
	public string Username { get; set; } = "root";
	
	[JsonPropertyName("Password")]
	public string Password { get; set; } = "";
	
	/// <summary>Telegram bot token for player reports. Leave empty to disable.</summary>
	[JsonPropertyName("TelegramBotToken")]
	public string TelegramBotToken { get; set; } = "";

	/// <summary>Telegram chat ID to receive reports. Leave empty to disable.</summary>
	[JsonPropertyName("TelegramChatId")]
	public string TelegramChatId { get; set; } = "";

	/// <summary>Predefined reasons for each action. Configurable via JSON.</summary>
	[JsonPropertyName("SlayReasons")]
	public List<string> SlayReasons { get; set; } = new()
	{
		"Teamkill",
		"AFK",
		"Cheating"
	};

	[JsonPropertyName("KickReasons")]
	public List<string> KickReasons { get; set; } = new()
	{
		"AFK",
		"Teamkill",
		"Toxic behavior",
		"Body blocking"
	};

	[JsonPropertyName("BanReasons")]
	public List<string> BanReasons { get; set; } = new()
	{
		"Cheating",
		"Griefing",
		"Ban evasion",
		"Toxic behavior"
	};

	[JsonPropertyName("GagReasons")]
	public List<string> GagReasons { get; set; } = new()
	{
		"Spam",
		"Toxic behavior",
		"Advertising"
	};

	[JsonPropertyName("MuteReasons")]
	public List<string> MuteReasons { get; set; } = new()
	{
		"Spam",
		"Toxic behavior",
		"Noise"
	};

	[JsonPropertyName("SilenceReason")]
	public List<string> SilenceReason { get; set; } = new()
	{
		"Spam",
		"Toxic behavior",
		"Noise"
	};

	[JsonPropertyName("BanDuration")]
	public Dictionary<string, int> BanDuration { get; set; } = new()
	{
		{ "30 minutes",  30 },
		{ "1 hour",      60 },
		{ "2 hours",     120 },
		{ "3 hours",     180 },
		{ "6 hours",     360 },
		{ "12 hours",    720 },
		{ "1 day",       1440 },
		{ "3 days",      4320 },
		{ "1 week",      10080 },
		{ "2 weeks",     20160 },
		{ "1 month",     43800 },
		{ "Permanent",   0 }
	};

	[JsonPropertyName("GagDuration")]
	public Dictionary<string, int> GagDuration { get; set; } = new()
	{
		{ "30 minutes",  30 },
		{ "1 hour",      60 },
		{ "2 hours",     120 },
		{ "3 hours",     180 },
		{ "6 hours",     360 },
		{ "12 hours",    720 },
		{ "1 day",       1440 },
		{ "3 days",      4320 },
		{ "1 week",      10080 },
		{ "2 weeks",     20160 },
		{ "1 month",     43800 },
		{ "Permanent",   0 }
	};

	[JsonPropertyName("MuteDuration")]
	public Dictionary<string, int> MuteDuration { get; set; } = new()
	{
		{ "30 minutes",  30 },
		{ "1 hour",      60 },
		{ "2 hours",     120 },
		{ "3 hours",     180 },
		{ "6 hours",     360 },
		{ "12 hours",    720 },
		{ "1 day",       1440 },
		{ "3 days",      4320 },
		{ "1 week",      10080 },
		{ "2 weeks",     20160 },
		{ "1 month",     43800 },
		{ "Permanent",   0 }
	};

	[JsonPropertyName("SilenceDuration")]
	public Dictionary<string, int> SilenceDuration { get; set; } = new()
	{
		{ "30 minutes",  30 },
		{ "1 hour",      60 },
		{ "2 hours",     120 },
		{ "3 hours",     180 },
		{ "6 hours",     360 },
		{ "12 hours",    720 },
		{ "1 day",       1440 },
		{ "3 days",      4320 },
		{ "1 week",      10080 },
		{ "2 weeks",     20160 },
		{ "1 month",     43800 },
		{ "Permanent",   0 }
	};
}