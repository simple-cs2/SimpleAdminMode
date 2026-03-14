// Models/GagEntry.cs
namespace SimpleAdminMode;

/// <summary>
/// Represents a gag record stored in sam_gags table.
/// A gag blocks a player's text chat.
/// </summary>
public class GagEntry
{
	public ulong    PlayerSteamID { get; set; }
	public string   PlayerName    { get; set; } = "";
	public ulong    AdminSteamID  { get; set; }
	public string   AdminName     { get; set; } = "";
	public string   Reason        { get; set; } = "";
	public DateTime CreatedAt     { get; set; }
	public DateTime ExpiredAt     { get; set; }
}