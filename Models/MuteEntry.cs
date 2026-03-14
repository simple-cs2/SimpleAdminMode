// Models/MuteEntry.cs
namespace SimpleAdminMode;

/// <summary>
/// Represents a mute record stored in sam_mutes table.
/// A mute blocks a player's voice chat.
/// </summary>
public class MuteEntry
{
	public ulong    PlayerSteamID { get; set; }
	public string   PlayerName    { get; set; } = "";
	public ulong    AdminSteamID  { get; set; }
	public string   AdminName     { get; set; } = "";
	public string   Reason        { get; set; } = "";
	public DateTime CreatedAt     { get; set; }
	public DateTime ExpiredAt     { get; set; }
}