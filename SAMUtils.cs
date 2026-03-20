// SAMUtils.cs
using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace SimpleAdminMode;

/// <summary>
/// Utility methods for SAM plugin — targeting, duration parsing and chat formatting.
/// </summary>
public static class SAMUtils
{
	// Max duration: 1 year in minutes
	private const int MaxDurationMinutes = 31536000;
	private static readonly (string Unit, long Minutes)[] DurationUnits =
	[
		("y",  525600L),
		("mo", 43800L),
		("w",  10080L),
		("d",  1440L),
		("h",  60L),
		("m",  1L)
	];

	private static readonly (string Name, int Minutes)[] FormatUnits =
	[
		(" year",   525600),
		(" month",  43800),
		(" week",   10080),
		(" day",    1440),
		(" hour",   60),
		(" minute", 1)
	];

	/// <summary>
	/// Parses a duration string into minutes.
	/// Supports formats like "1y2mo3d", "1h30m", "60" (raw minutes).
	/// Returns 0 for permament.
	/// </summary>
	public static int ParseDuration(string input)
	{
		input = input.Trim().ToLower();

		if(int.TryParse(input, out int directMinutes))
			return Math.Clamp(directMinutes, 0, MaxDurationMinutes);

		long minutes = 0;

		foreach(Match match in Regex.Matches(input, @"(\d{1,9})(y|mo|w|d|h|m)"))
		{
			long value = long.Parse(match.Groups[1].Value);
			string unit = match.Groups[2].Value;

			minutes += DurationUnits
				.FirstOrDefault(u => u.Unit == unit)
				.Minutes * value;
		}

		return (int)Math.Clamp(minutes, 0L, MaxDurationMinutes);
	}

	/// <summary>
	/// Formats a duration in minutes to a human-readable string.
	/// Returns "Indefinitely" for 0 or genative values.
	/// </summary>
	public static string FormatDuration(int minutes)
	{
		if(minutes <= 0) return "Indefinitely";
		if(minutes <= 1) return "1 minute";

		var parts = new List<string>();

		foreach(var (name, value) in FormatUnits)
		{
			int count = minutes / value;
			minutes %= value;

			if(count > 0)
				parts.Add($"{count}{name}{(count> 1 ? "s" : "")}");

			if(minutes == 0) break;
		}

		return parts.Count == 1
			? parts[0]
			: string.Join(", ", parts[..^1]) + " and " + parts[^1];
	}

	/// <summary>
	/// Resolves a target string to a list of players.
	/// Supports: ^ (self), * (all), name, SteamID64, #UserID.
	/// </summary>
	public static List<CCSPlayerController> GetTargets(CCSPlayerController admin, string target)
	{
		return target switch
		{
			"^" => [admin],
			"*" => Utilities.GetPlayers().ToList(),
			_   => Utilities.GetPlayers()
					.Where(p => p.PlayerName.Contains(target, StringComparison.OrdinalIgnoreCase)
						|| p.SteamID.ToString() == target
						|| $"#{p.UserId}" == target
					).ToList()
		};
	}

	/// <summary>
	/// Prints a formatted action message to all players in chat.
	/// </summary>
	public static void PrintActionToChat(
		CCSPlayerController? admin,
		string targetArg,
		List<CCSPlayerController>? targets,
		string actionPast,
		string optionalText = "")
	{
		if(admin == null) return;

		bool hasTargets = targets != null && targets.Count > 0;

		string who = hasTargets
			? (targetArg == "^" || (targets!.Count == 1 && targets[0].SteamID == admin.SteamID)
				? "Themselves"
				: targetArg == "*" || targets!.Count > 1
					? "Everyone"
					: targets!.FirstOrDefault()?.PlayerName ?? targetArg)
			: targetArg;

		Server.PrintToChatAll(
			$" {ChatColors.Red}[SAM] {ChatColors.Magenta}{admin.PlayerName} {ChatColors.Default}{actionPast} {ChatColors.Lime}{who}{optionalText}"
		);
	}
}