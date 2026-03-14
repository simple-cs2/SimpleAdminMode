// Models/BanEntry.cs
namespace SimpleAdminMode;

/// <summary>
/// Represents a ban record stored in sam_bans table.
/// </summary>
public class BanEntry
{
	public ulong    PlayerSteamID { get; set; }
	public string   PlayerName    { get; set; } = "";
	public ulong    AdminSteamID  { get; set; }
	public string   AdminName     { get; set; } = "";
	public string   Reason        { get; set; } = "";
	public DateTime CreatedAt     { get; set; }
	public DateTime ExpiredAt     { get; set; }
}