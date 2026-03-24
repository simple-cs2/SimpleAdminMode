// Models/WarnEntry.cs
namespace SimpleAdminMode;

/// <summary>
/// Represents a warn record stored in sam_warns table.
/// </summary>
public class WarnEntry
{
    public int      Id            { get; set; }
	public ulong    PlayerSteamID { get; set; }
	public string   PlayerName    { get; set; } = "";
	public ulong    AdminSteamID  { get; set; }
	public string   AdminName     { get; set; } = "";
	public string   Reason        { get; set; } = "";
	public DateTime CreatedAt     { get; set; } = DateTime.Now;
}