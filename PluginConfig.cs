using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace SimpleAdminMode;

/// <summary>
/// Plugin configuration — auto-generated at:
/// configs/plugins/SimpleAdminMode/SimpleAdminMode.json
/// </summary>
public class PluginConfig : BasePluginConfig
{
    // -------------------------------------------------------------------------
    // Database
    // -------------------------------------------------------------------------

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

    // -------------------------------------------------------------------------
    // Telegram
    // -------------------------------------------------------------------------

    /// <summary>Telegram bot token for player reports. Leave empty to disable.</summary>
    [JsonPropertyName("TelegramBotToken")]
    public string TelegramBotToken { get; set; } = "";

    /// <summary>Telegram chat ID to receive reports. Leave empty to disable.</summary>
    [JsonPropertyName("TelegramChatId")]
    public string TelegramChatId { get; set; } = "";

    // -------------------------------------------------------------------------
    // Warn system
    // -------------------------------------------------------------------------

    /// <summary>Maximum warns before auto-ban.</summary>
    [JsonPropertyName("MaxWarns")]
    public int MaxWarns { get; set; } = 3;

    /// <summary>Auto-ban duration in minutes after exceeding MaxWarns. 0 = permanent.</summary>
    [JsonPropertyName("WarnBanDuration")]
    public int WarnBanDuration { get; set; } = 60;

    /// <summary>Reason used for auto-ban when warns exceed limit.</summary>
    [JsonPropertyName("WarnBanReason")]
    public string WarnBanReason { get; set; } = "Exceeded warn limit";

    // -------------------------------------------------------------------------
    // Predefined reasons
    // -------------------------------------------------------------------------

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

    [JsonPropertyName("WarnReasons")]
    public List<string> WarnReasons { get; set; } = new()
    {
        "Toxic behavior",
        "Spam",
        "Cheating",
        "Griefing",
        "Advertising"
    };

    // -------------------------------------------------------------------------
    // Predefined durations
    // -------------------------------------------------------------------------

    private static Dictionary<string, int> DefaultDurations => new()
    {
        { "30 minutes", 30    },
        { "1 hour",     60    },
        { "2 hours",    120   },
        { "3 hours",    180   },
        { "6 hours",    360   },
        { "12 hours",   720   },
        { "1 day",      1440  },
        { "3 days",     4320  },
        { "1 week",     10080 },
        { "2 weeks",    20160 },
        { "1 month",    43800 },
        { "Permanent",  0     }
    };

    [JsonPropertyName("BanDuration")]
    public Dictionary<string, int> BanDuration { get; set; } = DefaultDurations;

    [JsonPropertyName("GagDuration")]
    public Dictionary<string, int> GagDuration { get; set; } = DefaultDurations;

    [JsonPropertyName("MuteDuration")]
    public Dictionary<string, int> MuteDuration { get; set; } = DefaultDurations;

    [JsonPropertyName("SilenceDuration")]
    public Dictionary<string, int> SilenceDuration { get; set; } = DefaultDurations;
}