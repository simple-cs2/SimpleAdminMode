// Database/BanRepository.cs
using Dapper;
using MySqlConnector;

namespace SimpleAdminMode;

/// <summary>
/// Handles all database operations for the sam_bans table.
/// </summary>
public class BanRepository
{
	private readonly string _connectionString;
	private MySqlConnection CreateConnection() => new(_connectionString);

	public BanRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	/// <summary>Adds a new ban record.</summary>
	public async Task AddAsync(BanEntry ban)
	{
		using var connection = CreateConnection();
		await connection.ExecuteAsync(@"
			INSERT INTO sam_bans (player_steamid, player_name, admin_steamid, admin_name, reason, created_at, expired_at)
			VALUES (@PlayerSteamID, @PlayerName, @AdminSteamID, @AdminName, @Reason, @CreatedAt, @ExpiredAt)",
			ban
		);
	}

	/// <summary>Returns an active ban for the given SteamID, or null if not found.</summary>
	public async Task<BanEntry?> GetActiveAsync(ulong steamId)
	{
		using var connection = CreateConnection();
		return await connection.QueryFirstOrDefaultAsync<BanEntry>(@"
			SELECT * FROM sam_bans WHERE steam_id = @SteamId AND expired_at > NOW()",
			new { SteamId = steamId }
		);
	}

	/// <summary>Removes all active bans for the given SteamID.</summary>
	public async Task RemoveAsync(ulong steamId)
	{
		using var connection = CreateConnection();
		await connection.ExecuteAsync(@"
			DELETE FROM sam_bans WHERE steam_id = @SteamId",
			new { SteamId = steamId }
		);
	}
}