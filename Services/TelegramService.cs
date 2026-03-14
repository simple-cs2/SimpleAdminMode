// Services/TelegramService.cs
using System.Net.Http;
using System.Web;

namespace SimpleAdminMode;

/// <summary>
/// Handles sending notifications to a Telegram chat via Bot API.
/// </summary>
public class TelegramService
{
	private readonly string _botToken;
	private readonly string _chatId;

	// Reuse HttpClient across requests — avoids socket exhaustion
	private static readonly HttpClient _httpClient = new();
	public bool IsConfigured => !string.IsNullOrEmpty(_botToken) && !string.IsNullOrEmpty(_chatId);

	public TelegramService(string botToken, string chatId)
	{
		_botToken = botToken;
		_chatId = chatId;
	}

	/// <summary>
	/// Sends a player report to the configured Telegram chat.
	/// </summary>
	public async Task SendReportAsync(
		string reporterName, ulong reporterSteamId,
		string targetName, ulong targetSteamId,
		string reason, string map)
	{
		string message =
			$"🚨 <b>[SAM] New Report</b>\n\n"                              +
			$"👤 <b>Reporter:</b> {reporterName} (<code>{reporterSteamId}</code>)\n" +
			$"🎯 <b>Target:</b> {targetName} (<code>{targetSteamId}</code>)\n"       +
			$"📝 <b>Reason:</b> {reason}\n"                                +
			$"🗺️ <b>Map:</b> {map}\n"                                      +
			$"⏰ <b>Time:</b> {DateTime.Now:yyyy-MM-dd HH:mm}";

		string url = $"https://api.telegram.org/bot{_botToken}/sendMessage"  +
					 $"?chat_id={_chatId}"                                    +
					 $"&text={HttpUtility.UrlEncode(message)}"                +
					 $"&parse_mode=HTML";

		try
		{
			var response = await _httpClient.GetAsync(url);
			string body  = await response.Content.ReadAsStringAsync();

			Console.ForegroundColor = response.IsSuccessStatusCode ? ConsoleColor.Green : ConsoleColor.Red;
			Console.WriteLine($"[SAM] Telegram: {(int)response.StatusCode} — {body}");
			Console.ResetColor();
		}
		catch(Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"[SAM] Telegram error: {ex.Message}");
			Console.ResetColor();
		}
	}
}