# 🛡️ SAM — Simple Admin Mode

![GitHub release](https://img.shields.io/github/v/release/simple-cs2/SimpleAdminMode)
![GitHub stars](https://img.shields.io/github/stars/simple-cs2/SimpleAdminMode)
![GitHub issues](https://img.shields.io/github/issues/simple-cs2/SimpleAdminMode)
![License](https://img.shields.io/github/license/simple-cs2/SimpleAdminMode)
![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-1.0.363+-blue)

> A lightweight and powerful admin system for CS2 servers built on CounterStrikeSharp.

SAM provides a simple yet powerful administration system with punishments,
communication control, admin menu integration, and MySQL storage.

It is designed to be fast, modular, and easy to integrate into existing CS2 servers.

## 📦 Dependencies

- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) `v1.0.363+`
- [MenuManager](https://github.com/Stimayk/MenuManagerCS2) `v1.1.1+`
- MySQL `5.7+` or MariaDB `10.3+`


## ⚙️ Installation

1. Download the latest release from [Releases](../../releases)
2. Extract `SimpleAdminMode` folder to:
```
   csgo/addons/counterstrikesharp/plugins/
```
3. Download **MenuManagerCore.zip** from:  
https://github.com/Stimayk/MenuManagerCS2/releases

Extract it to:
```
   csgo/addons/counterstrikesharp/plugins/
```
4. Download **MenuManagerApi.zip** from the same release.

Extract `MenuManagerApi.dll` to:
```
   csgo/addons/counterstrikesharp/shared/
```
5. Configure `SimpleAdminMode.json` (see Configuration)
6. Restart your server

## 🔧 Configuration

`csgo/addons/counterstrikesharp/configs/plugins/SimpleAdminMode/SimpleAdminMode.json`
```json
{
  "Host": "127.0.0.1",
  "Port": 3306,
  "Database": "sam",
  "Username": "root",
  "Password": "",
  "TelegramBotToken": "",
  "TelegramChatId": ""
}
```

| Field | Description |
|---|---|
| `Host` | MySQL server host |
| `Port` | MySQL server port |
| `Database` | Database name |
| `Username` | MySQL username |
| `Password` | MySQL password |
| `TelegramBotToken` | Telegram bot token for reports |
| `TelegramChatId` | Telegram chat ID for reports |

## 📋 Commands

### 🔨 Punishment
| Command | Permission | Description |
|---|---|---|
| `!slay <target>` | `@css/slay` | Kill a player |
| `!kick <target> [reason]` | `@css/kick` | Kick a player |
| `!ban <target> <duration> [reason]` | `@css/ban` | Ban a player |
| `!unban <steamid64>` | `@css/ban` | Unban a player |

### 🔇 Communication
| Command | Permission | Description |
|---|---|---|
| `!mute <target> <duration> [reason]` | `@css/chat` | Block voice chat |
| `!unmute <steamid64>` | `@css/chat` | Unblock voice chat |
| `!gag <target> <duration> [reason]` | `@css/chat` | Block text chat |
| `!ungag <steamid64>` | `@css/chat` | Unblock text chat |
| `!silence <target> <duration> [reason]` | `@css/chat` | Block all communication |
| `!unsilence <steamid64>` | `@css/chat` | Unblock all communication |

### 🛠️ Other
| Command | Permission | Description |
|---|---|---|
| `!rename <target> <name>` | `@css/rename` | Rename a player |
| `!report <target> <reason>` | — | Report a player to Telegram |
| `!admin` | `@css/generic` | Open admin menu |

### 🎯 Targeting
| Key | Description |
|---|---|
| `*` | All players |
| `^` | Yourself |
| `<name>` | Player by name |
| `<userid>` | Player by UserId |
| `<steamid64>` | Player by SteamID64 |

### ⏱️ Duration Format
```
!ban player 1h30m cheating
!ban player 1d12h griefing
!ban player 1mo ban evading
!ban player 0 permanent ban

1m  = 1 minute
1h  = 1 hour
1d  = 1 day
1w  = 1 week
1mo = 1 month
1y  = 1 year
```

## 🤖 Telegram Reports

SAM can send player reports to a Telegram chat.

### 1. Create a bot

Open Telegram and start a chat with **@BotFather**.

Run the command:
```
/newbot
```

Follow the instructions and copy the bot token.

### 2. Get your Chat ID

1. Add the bot to your Telegram group or chat.
2. Send a message in the chat.
3. Open in your browser:
```
https://api.telegram.org/bot<BOT_TOKEN>/getUpdates
```
Find `"chat":{"id":...}` in the response and copy the ID.

### 3. Configure the plugin

Put the values in `SimpleAdminMode.json`:

```json
{
  "TelegramBotToken": "your_bot_token",
  "TelegramChatId": "your_chat_id"
}
```

---

## 📸 Screenshots

> Coming soon!

---

## 👤 Author

Made with ❤️ by [kotyarakryt](https://t.me/kotyarakryt)