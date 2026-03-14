// Commands/SilenceCommand.cs
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace SimpleAdminMode;

public partial class SimpleAdminMode
{
	/// <summary>
	/// !silence &lt;target&gt; &lt;duration&gt; [reason] — Blocks both voice and text chat.
	/// </summary>
	private void OnSilenceCommand(CCSPlayerController? player, CommandInfo command)
	{
		OnGagCommand(player, command);
		OnMuteCommand(player, command);
	}

	/// <summary>
	/// !unsilence &lt;steamid64&gt; — Unblocks both voice and text chat.
	/// </summary>
	private void OnUnSilenceCommand(CCSPlayerController? player, CommandInfo command)
	{
		OnUnGagCommand(player, command);
		OnUnMuteCommand(player, command);
	}
}