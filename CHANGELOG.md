# Changelog

All notable changes to this project will be documented in this file.

## [0.1.0] - 2026-03-14
### Added
- Initial project setup

## [0.2.0] - 2026-03-24
### Added
- Admin menu with full MenuManager integration
  - `AdminMenu.cs` with complete flow: player → duration → reason
  - Custom duration/reason input via chat with `!cancel` support
  - Predefined reasons and durations per action in `PluginConfig`
  - `SilenceReasons` list in `PluginConfig`
- Freeze and unfreeze commands
- Warn system with auto-ban and notifications
  - `WarnEntry` model and `WarnRepository`
  - `sam_warns` table in database
  - `!warn`, `!unwarn`, `!warnlist` commands
  - CenterHtml notification for warn events
  - Auto-ban after `MaxWarns` with configurable duration
  - Warn timer cache to prevent duplicate notifications
- Warn auto-ban settings
  - `MaxWarns` (maximum warns before auto-ban)
  - `WarnBanDuration` (ban duration in minutes, 0 = permanent)
  - `WarnBanReason` (reason for auto-ban when limit is exceeded)
- Support for console as action sender in `PrintActionToChat`

### Fixed
- Uncommented `Server.ExecuteCommand` for kick/ban functionality

### Changed
- Minor code edits and tabulation fixes
- Removed development placeholder messages from `AdminMenuCommand.cs`

### Style
- Fixed indentation and removed redundant comments