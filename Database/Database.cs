// Database/Database.cs
using MySqlConnector;
using Dapper;

namespace SimpleAdminMode;

/// <summary>
/// Main database context. Provides access to all repositories
/// and handles table initialization on plugin load.
/// </summary>
public class Database
{
	private readonly string _connectionString;

	public BanRepository  Bans  { get; }
	public GagRepository  Gags  { get; }
	public MuteRepository Mutes { get; }
	public WarnRepository Warns { get; }

	public Database(string connectionString)
	{
		_connectionString = connectionString;

		// Enable automatic snake_case → PascalCase mapping for all queries
		DefaultTypeMap.MatchNamesWithUnderscores = true;

		Bans  = new BanRepository(connectionString);
		Gags  = new GagRepository(connectionString);
		Mutes = new MuteRepository(connectionString);
		Warns = new WarnRepository(connectionString);
	}

	public MySqlConnection CreateConnection() => new(_connectionString);

	/// <summary>
	/// Creates all required tables if they don't exist.
	/// Called once on plugin load.
	/// </summary>
	public async Task InitializeAsync()
	{
		using var connection = CreateConnection();
		await connection.OpenAsync();

		await connection.ExecuteAsync(@"
			CREATE TABLE IF NOT EXISTS sam_bans (
				id              INT AUTO_INCREMENT PRIMARY KEY,
				player_steamid  BIGINT UNSIGNED NOT NULL,
				player_name     VARCHAR(64)     NOT NULL,
				admin_steamid   BIGINT UNSIGNED NOT NULL,
				admin_name      VARCHAR(64)     NOT NULL,
				reason          VARCHAR(255)    NOT NULL,
				created_at      DATETIME        NOT NULL,
				expired_at      DATETIME        NOT NULL,
				INDEX idx_player_steamid (player_steamid),
				INDEX idx_expired_at     (expired_at)
			);
			CREATE TABLE IF NOT EXISTS sam_gags (
				id              INT AUTO_INCREMENT PRIMARY KEY,
				player_steamid  BIGINT UNSIGNED NOT NULL,
				player_name     VARCHAR(64)     NOT NULL,
				admin_steamid   BIGINT UNSIGNED NOT NULL,
				admin_name      VARCHAR(64)     NOT NULL,
				reason          VARCHAR(255)    NOT NULL,
				created_at      DATETIME        NOT NULL,
				expired_at      DATETIME        NOT NULL,
				INDEX idx_player_steamid (player_steamid),
				INDEX idx_expired_at     (expired_at)
			);
			CREATE TABLE IF NOT EXISTS sam_mutes (
				id              INT AUTO_INCREMENT PRIMARY KEY,
				player_steamid  BIGINT UNSIGNED NOT NULL,
				player_name     VARCHAR(64)     NOT NULL,
				admin_steamid   BIGINT UNSIGNED NOT NULL,
				admin_name      VARCHAR(64)     NOT NULL,
				reason          VARCHAR(255)    NOT NULL,
				created_at      DATETIME        NOT NULL,
				expired_at      DATETIME        NOT NULL,
				INDEX idx_player_steamid (player_steamid),
				INDEX idx_expired_at     (expired_at)
			);
			CREATE TABLE IF NOT EXISTS sam_warns (
				id              INT AUTO_INCREMENT PRIMARY KEY,
				player_steamid  BIGINT UNSIGNED NOT NULL,
				player_name     VARCHAR(64)     NOT NULL,
				admin_steamid   BIGINT UNSIGNED NOT NULL,
				admin_name      VARCHAR(64)     NOT NULL,
				reason          VARCHAR(255)    NOT NULL,
				created_at      DATETIME        NOT NULL,
				INDEX idx_player_steamid (player_steamid)
			);
		");

	}
}