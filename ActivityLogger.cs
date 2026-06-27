using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    /// <summary>
    /// Tracks all bot/user actions during the session.
    /// Stores a timestamped list of activity entries.
    /// </summary>
    public static class ActivityLogger
    {
        private static readonly List<string> _log = new List<string>();

        /// <summary>Log a new activity entry with a timestamp.</summary>
        public static void Log(string message)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _log.Insert(0, entry); // Newest first
        }

        /// <summary>Get recent entries (up to a limit).</summary>
        public static List<string> GetRecent(int count = 5)
        {
            int take = Math.Min(count, _log.Count);
            return _log.GetRange(0, take);
        }

        /// <summary>Get ALL log entries.</summary>
        public static List<string> GetAll() => new List<string>(_log);

        /// <summary>Total number of log entries.</summary>
        public static int Count => _log.Count;

        /// <summary>Clear the log.</summary>
        public static void Clear() => _log.Clear();
    }
}
