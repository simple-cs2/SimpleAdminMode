// Database/GagRepository.cs
using Dapper;
using MySqlConnector;

namespace SimpleAdminMode;

/// <summary>
/// Handles all database operations for the sam_gags table.
/// </summary>
public class GagRepository
{
	private readonly string _connectionString;
	private MySqlConnection CreateConnection() => new(_connectionString);

	public GagRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	/// <summary>Adds a new gag record.</summary>
	public async Task AddAsync(GagEntry mute)
	{
		using var connection = CreateConnection();
		await connection.ExecuteAsync(@"
			INSERT INTO sam_gags (player_steamid, player_name, admin_steamid, admin_name, reason, created_at, expired_at)
			VALUES (@PlayerSteamID, @PlayerName, @AdminSteamID, @AdminName, @Reason, @CreatedAt, @ExpiredAt)",
			mute
		);
	}

	/// <summary>Returns an active gag for the given SteamID, or null if not found.</summary>
	public async Task<GagEntry?> GetActiveAsync(ulong steamId)
	{
		using var connection = CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<GagEntry>(@"
			SELECT * FROM sam_gags WHERE player_steamid = @SteamId AND expired_at > NOW();",
			new { SteamId = steamId }
		);
	}

	/// <summary>Returns all currently active gags. Used for cache initialization on plugin load.</summary>
	public async Task<List<GagEntry>> GetAllActiveAsync()
	{
		using var connection = CreateConnection();
		var result = await connection.QueryAsync<GagEntry>(@"SELECT * FROM sam_gags");
		return result.Where(m => m.ExpiredAt > DateTime.Now).ToList();
	}

	/// <summary>Removes all active gags for the given SteamID.</summary>
	public async Task RemoveAsync(ulong steamId)
	{
		using var connection = CreateConnection();
		await connection.ExecuteAsync(@"
			DELETE FROM sam_gags WHERE player_steamid = @SteamId",
			new { SteamId = steamId }
		);
	}
}