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
}