using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CybersecurityChatbotGUI
{
    /// <summary>
    /// Handles all MySQL database operations for the Task Assistant.
    /// Connection string targets localhost with the cyberbot_db database.
    /// </summary>
    public static class DatabaseHelper
    {
        // ── Connection String ─────────────────────────────────────────────────
        private const string ConnectionString =
            "Server=localhost;Database=cyberbot_db;Uid=root;Pwd=;";

        // ── Task Model ────────────────────────────────────────────────────────

        public class TaskItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";
            public string ReminderDate { get; set; } = "";
            public bool IsCompleted { get; set; }
            public string CreatedAt { get; set; } = "";
        }

        // ── CRUD Operations ───────────────────────────────────────────────────

        /// <summary>Add a new task to the database.</summary>
        public static bool AddTask(string title, string description, string reminderDate)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = "INSERT INTO tasks (title, description, reminder_date) VALUES (@title, @desc, @reminder)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@reminder", reminderDate);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"DB Error (Add): {ex.Message}", "Database Error");
                return false;
            }
        }

        /// <summary>Retrieve all tasks from the database.</summary>
        public static List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = "SELECT id, title, description, reminder_date, is_completed, created_at FROM tasks ORDER BY created_at DESC";
                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tasks.Add(new TaskItem
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                        ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date")) ? "" : reader.GetString("reminder_date"),
                        IsCompleted = reader.GetInt32("is_completed") == 1,
                        CreatedAt = reader.GetDateTime("created_at").ToString("yyyy-MM-dd HH:mm")
                    });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"DB Error (Get): {ex.Message}", "Database Error");
            }
            return tasks;
        }

        /// <summary>Mark a task as completed.</summary>
        public static bool MarkTaskCompleted(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = "UPDATE tasks SET is_completed = 1 WHERE id = @id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"DB Error (Update): {ex.Message}", "Database Error");
                return false;
            }
        }

        /// <summary>Delete a task from the database.</summary>
        public static bool DeleteTask(int taskId)
        {
            try
            {
                using var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                string sql = "DELETE FROM tasks WHERE id = @id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", taskId);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"DB Error (Delete): {ex.Message}", "Database Error");
                return false;
            }
        }
    }
}