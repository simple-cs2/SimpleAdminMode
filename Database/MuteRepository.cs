// Database/MuteRepository.cs
using Dapper;
using MySqlConnector;

namespace SimpleAdminMode;

/// <summary>
/// Handles all database operations for the sam_mutes table.
/// </summary>
public class MuteRepository
{
	private readonly string _connectionString;
	private MySqlConnection CreateConnection() => new(_connectionString);

	public MuteRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	/// <summary>Adds a new mute record.</summary>
	public async Task AddAsync(MuteEntry mute)
	{
		using var connection = CreateConnection();
		await connection.ExecuteAsync(@"
			INSERT INTO sam_mutes (player_steamid, player_name, admin_steamid, admin_name, reason, created_at, expired_at)
			VALUES (@PlayerSteamID, @PlayerName, @AdminSteamID, @AdminName, @Reason, @CreatedAt, @ExpiredAt)",
			mute
		);
	}

	/// <summary>Returns an active mute for the given SteamID, or null if not found.</summary>
	public async Task<MuteEntry?> GetActiveAsync(ulong steamId)
	{
		using var connection = CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<MuteEntry>(@"
			SELECT * FROM sam_mutes WHERE player_steamid = @SteamId AND expired_at > NOW();",
			new { SteamId = steamId }
		);
	}

	/// <summary>Returns all currently active mutes. Used for cache initialization on plugin load.</summary>
	public async Task<List<MuteEntry>> GetAllActiveAsync()
	{
		using var connection = CreateConnection();
		var result = await connection.QueryAsync<MuteEntry>(@"SELECT * FROM sam_mutes");
		return result.Where(m => m.ExpiredAt > DateTime.Now).ToList();
	}

	/// <summary>Removes all active mutes for the given SteamID.</summary>
	public async Task RemoveAsync(ulong steamId)
	{
		using var connection = CreateConnection();
		await connection.ExecuteAsync(@"
			DELETE FROM sam_mutes WHERE player_steamid = @SteamId",
			new { SteamId = steamId }
		);
	}
}