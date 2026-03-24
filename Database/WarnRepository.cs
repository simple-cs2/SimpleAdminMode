// Database/WarnRepository.cs
using Dapper;
using MySqlConnector;

namespace SimpleAdminMode;

/// <summary>
/// Handles all database operations for the sam_warns table.
/// </summary>
public class WarnRepository
{
	private readonly string _connectionString;
	private MySqlConnection CreateConnection() => new(_connectionString);

	public WarnRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	/// <summary>Adds a new warn record.</summary>
	public async Task AddAsync(WarnEntry warn)
	{
		using var connection = CreateConnection();
		await connection.ExecuteAsync(@"
			INSERT INTO sam_warns (player_steamid, player_name, admin_steamid, admin_name, reason, created_at)
			VALUES (@PlayerSteamID, @PlayerName, @AdminSteamID, @AdminName, @Reason, @CreatedAt)",
			warn
		);
	}

	/// <summary>Returns all warns for a player.</summary>
	public async Task<List<WarnEntry>> GetAllAsync(ulong steamId)
	{
		using var connection = CreateConnection();
		var result = await connection.QueryAsync<WarnEntry>(@"
			SELECT * FROM sam_warns
			WHERE player_steamid = @SteamId
			ORDER BY created_at DESC",
			new { SteamId = steamId }
		);
		return result.ToList();
	}

    /// <summary>Returns active warn count for a player.</summary>
    public async Task<int> GetCountAsync(ulong steamId)
    {
        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(@"
            SELECT COUNT(*) FROM sam_warns
            WHERE player_steamid = @SteamId",
            new { SteamId = steamId }
        );
    }

    /// <summary>Removes the most recent warn for a player.</summary>
    public async Task RemoveLastAsync(ulong steamId)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(@"
            DELETE FROM sam_warns
            WHERE player_steamid = @SteamId
            ORDER BY created_at DESC
            LIMIT 1",
            new { SteamId = steamId }
        );
    }

    /// <summary>Removes all warns for a player.</summary>
    public async Task RemoveAllAsync(ulong steamId)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(@"
            DELETE FROM sam_warns
            WHERE player_steamid = @SteamId",
            new { SteamId = steamId }
        );
    }
}