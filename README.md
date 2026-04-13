CyberBot — Cybersecurity Awareness Chatbot

 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [How to Run](#how-to-run)
- [Voice Greeting Setup](#voice-greeting-setup)
- [Topics Covered](#topics-covered)
- [Input Validation](#input-validation)
- [GitHub Actions CI](#github-actions-ci)
- [CI Screenshot](#ci-screenshot)
- [Commit History](#commit-history)

---

## Overview

CyberBot is a Part 1 assignment submission for the Cybersecurity Awareness Bot project. It is built using C# (.NET 8) as a console application that provides users with educational cybersecurity information through a friendly, interactive chat interface.

The bot features a voice greeting, ASCII art logo, personalised responses, input validation, and an enhanced console UI with colour formatting and a typing effect.

---

## Features

| Feature | Description |
|---|---|
| 🎙️ Voice Greeting | Plays a WAV audio file when the application launches |
| 🖼️ ASCII Art Logo | Displays a styled CYBERBOT header on startup |
| 👤 Personalised Chat | Asks for the user's name and uses it throughout the session |
| ✅ Input Validation | Handles empty input, single characters, long input, and unknown queries |
| 🛡️ 12 Cybersecurity Topics | Covers passwords, phishing, malware, VPN, 2FA, WiFi, backups, and more |
| 🎨 Enhanced Console UI | Coloured text, typing animation, section dividers, and warning messages |
| 📝 XML Documentation | Every class and method includes full XML summary documentation |
| 🔄 GitHub Actions CI | Automated build and file structure verification on every push |

---

## Project Structure

```
CyberBot/
│
├── Program.cs                        ← Entry point (starts the bot)
│
├── Core/
│   └── ChatbotEngine.cs              ← Main chat loop and input validation
│
├── UI/
│   └── ConsoleUI.cs                  ← All console output, colours, ASCII art
│
├── Responses/
│   └── ResponseHandler.cs            ← All cybersecurity Q&A topic responses
│
├── Media/
│   └── AudioPlayer.cs                ← WAV audio playback (Windows/Linux/macOS)
│
├── greeting.wav                      ← Voice greeting audio file
│
├── CyberBot.csproj                   ← .NET 8 project file
│
└── .github/
    └── workflows/
        └── ci.yml                    ← GitHub Actions CI workflow
```

### Why is it structured this way?

The project is split into separate classes on purpose:

- **Program.cs** only starts the app — no logic lives here.
- **ChatbotEngine** owns the chat loop and all input validation.
- **ConsoleUI** owns all formatting — no other class writes directly to the console.
- **ResponseHandler** only returns strings — it never touches the console.
- **AudioPlayer** handles all OS detection and audio — isolated from everything else.

This makes the code easy to read, test, and maintain.

---

## How to Run

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later
- Visual Studio 2022 (recommended) or any terminal

### Run in Visual Studio 2022

1. Open the `CyberBot` solution in Visual Studio 2022
2. Press **Ctrl + F5** (Run without debugging)
3. The console window will open and the bot will start

### Run in Terminal

```bash
cd CyberBot
dotnet run
```

### Build Only

```bash
dotnet build CyberBot.csproj --configuration Release
```

---

## Voice Greeting Setup

1. Record a short voice message, for example:

   *"Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online."*

2. Save the file as **greeting.wav**

3. Place it in the same folder as `Program.cs`

4. The bot will automatically detect and play it on startup

> If the file is missing, the bot starts silently without any errors.

| Platform | Playback Method |
|---|---|
| Windows | System.Media.SoundPlayer |
| Linux | aplay or paplay |
| macOS | afplay |

---

## Topics Covered

| Topic | Example keywords to type |
|---|---|
| 🔑 Passwords | password, passphrase, credentials |
| 🎣 Phishing | phishing, fake email, scam |
| 🌐 Safe Browsing | browsing, website, https |
| 🦠 Malware | malware, virus, ransomware, trojan |
| 🛡️ VPN | vpn, virtual private network |
| 🔐 2FA / MFA | two factor, 2fa, authenticator |
| 📶 WiFi Safety | wifi, public wifi, hotspot |
| 💾 Backups | backup, recovery, restore |
| ⚙️ Updates | update, patch, firmware |
| 🧠 Social Engineering | social engineering, pretexting |
| 🔒 Encryption | encryption, encrypt, cipher |
| 👁️ Privacy | privacy, personal data, popia |

You can also type:
- `help` — to see the full topic menu
- `how are you` — for a conversational response
- `what is your purpose` — to learn what the bot does
- `exit` or `quit` — to close the bot

---

## Input Validation

| Input | Bot Response |
|---|---|
| Empty / blank input | "I didn't receive any input. Please type something." |
| Single character | "That's too brief. Please ask a full question." |
| Input over 300 characters | "That message is too long. Please keep it short." |
| Unrecognised topic | "I didn't quite understand that. Could you rephrase?" |

---

## GitHub Actions CI

The CI workflow (`.github/workflows/ci.yml`) runs automatically on every push and pull request to `main` or `master`.

Steps performed:

1. ✅ Checkout the repository
2. ✅ Set up .NET 8
3. ✅ Restore dependencies
4. ✅ Build in Release mode — catches all syntax and compile errors
5. ✅ Verify all required source files are present
6. ✅ Print a success summary

---

## CI Screenshot

> Add your screenshot here after your first successful GitHub Actions run.
> Go to the Actions tab in your GitHub repo, wait for the green checkmark,
> take a screenshot, save it as ci-screenshot.png in your project folder, then replace this line with:
> ![CI passing](ci-screenshot.png)

---

## Commit History

| # | Commit Message |
|---|---|
| 1 | Initial commit: Set up CyberBot project structure and main files |
| 2 | feat: Add ASCII art header and ConsoleUI with typing effect |
| 3 | feat: Implement cross-platform WAV voice greeting via AudioPlayer |
| 4 | feat: Add ResponseHandler with 12 cybersecurity topic responses |
| 5 | feat: Add multi-layer input validation in ChatbotEngine |
| 6 | feat: Add GitHub Actions CI workflow for automated build checks |
| 7 | docs: Add README with full project documentation |

---

## Technologies Used

- **Language:** C# (.NET 8)
- **IDE:** Visual Studio 2022
- **Audio:** System.Media.SoundPlayer (Windows) / aplay / afplay
- **CI/CD:** GitHub Actions
- **Version Control:** Git / GitHub

---


