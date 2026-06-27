# 🛡️ CyberBot v3.0 — Cybersecurity Awareness Chatbot

## Setup Instructions
1. Install Visual Studio 2022 with .NET 8 and WPF support
2. Install XAMPP and start MySQL
3. Open phpMyAdmin and run:
```sql
   CREATE DATABASE cyberbot_db;
   USE cyberbot_db;
   CREATE TABLE tasks (
       id INT AUTO_INCREMENT PRIMARY KEY,
       title VARCHAR(200) NOT NULL,
       description TEXT,
       reminder_date VARCHAR(100),
       is_completed TINYINT(1) DEFAULT 0,
       created_at DATETIME DEFAULT CURRENT_TIMESTAMP
   );
```
4. Open `CybersecurityChatbotGUI.sln` in Visual Studio
5. Restore NuGet packages (MySql.Data)
6. Press F5 to run

## Features
- 💬 Chat: NLP-powered cybersecurity awareness chatbot
- ✅ Task Assistant: Add, view, complete, delete tasks saved to MySQL
- 🎮 Quiz: 12 cybersecurity questions with scoring and feedback
- 📋 Activity Log: Full session log paginated 5 at a time
- 🧠 NLP: 30+ keyword patterns for natural conversation
- 😊 Sentiment Detection: Live mood indicator
- 🔊 Voice Greeting: Plays on startup

## YouTube Presentation
[Insert your YouTube unlisted link here]

## GitHub
https://github.com/St10456708/CybersecurityChatbot