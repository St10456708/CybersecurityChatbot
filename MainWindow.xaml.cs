using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CybersecurityChatbotGUI
{
    public partial class MainWindow : Window
    {
        // ── Fields ─────────────────────────────────────────────────────────────
        private string _userName = "User";
        private readonly Memory _memory = new Memory();
        private bool _chatStarted = false;
        private string? _lastResponseTopic = null;

        // Quiz state
        private List<QuizData.QuizQuestion>? _quizQuestions;
        private int _quizCurrentIndex = 0;
        private int _quizScore = 0;
        private bool _quizAnswered = false;

        // Task state: waiting for reminder?
        private bool _awaitingReminder = false;
        private string _pendingTaskTitle = "";
        private string _pendingTaskDesc = "";

        // Activity log: how many to show
        private int _logShowCount = 5;

        // ── Constructor ─────────────────────────────────────────────────────────
        public MainWindow()
        {
            InitializeComponent();
        }

        // ── Window Loaded ───────────────────────────────────────────────────────
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
            AddBotMessage("🛡️ Welcome to CyberBot v3.0!\n\nI am your personal digital guardian — here to help you stay safe online.\n\nNEW in v3.0: Task Assistant 📋 | Quiz Game 🎮 | Activity Log 📊 | Advanced NLP 🧠\n\nPlease enter your name above to begin.", isGreeting: true);
            ActivityLogger.Log("CyberBot v3.0 started.");
        }

        // ── Voice Greeting ──────────────────────────────────────────────────────
        private void PlayVoiceGreeting()
        {
            try
            {
                string wavPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(wavPath))
                {
                    var player = new SoundPlayer(wavPath);
                    player.LoadCompleted += (s, e) => player.Play();
                    player.LoadAsync();
                }
            }
            catch { }
        }

        // ── Name Entry ──────────────────────────────────────────────────────────
        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BeginChat();
        }

        private void StartChat_Click(object sender, RoutedEventArgs e) => BeginChat();

        private void BeginChat()
        {
            string name = NameInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                NameInput.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }
            _userName = char.ToUpper(name[0]) + name.Substring(1);
            _memory.Store("user_name", _userName);
            _chatStarted = true;

            NameBar.Visibility = Visibility.Collapsed;
            AsciiSplash.Visibility = Visibility.Collapsed;
            UserInput.IsEnabled = true;
            SendBtn.IsEnabled = true;
            EnableChips(true);
            UserInput.Focus();

            ActivityLogger.Log($"Chat session started for user: {_userName}");

            AddBotMessage(
                $"Welcome, {_userName}! 🛡️\n\n" +
                $"I'm CyberBot v3.0 — your Cybersecurity Awareness Bot.\n\n" +
                $"Here's what you can do:\n" +
                $"💬 Chat — Ask me about cybersecurity topics\n" +
                $"✅ Task Assistant — Manage your security tasks (saved to database!)\n" +
                $"🎮 Quiz — Test your cybersecurity knowledge\n" +
                $"📋 Activity Log — See what you've done this session\n\n" +
                $"You can also type: 'add task', 'start quiz', or 'show log' right here! 👇");

            AddBotMessage(Responses.GetRandomTip(), isTip: true);
        }

        // ── TAB NAVIGATION ──────────────────────────────────────────────────────
        private void TabChat_Click(object sender, RoutedEventArgs e)
        {
            ShowTab("chat");
            ActivityLogger.Log("Opened Chat tab.");
        }

        private void TabTask_Click(object sender, RoutedEventArgs e)
        {
            if (!_chatStarted) return;
            ShowTab("task");
            RefreshTaskList();
            ActivityLogger.Log("Opened Task Assistant tab.");
        }

        private void TabQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (!_chatStarted) return;
            ShowTab("quiz");
            ActivityLogger.Log("Opened Quiz tab.");
        }

        private void TabLog_Click(object sender, RoutedEventArgs e)
        {
            if (!_chatStarted) return;
            ShowTab("log");
            RefreshActivityLog();
        }

        private void ShowTab(string tab)
        {
            ChatTab.Visibility = tab == "chat" ? Visibility.Visible : Visibility.Collapsed;
            TaskTab.Visibility = tab == "task" ? Visibility.Visible : Visibility.Collapsed;
            QuizTab.Visibility = tab == "quiz" ? Visibility.Visible : Visibility.Collapsed;
            LogTab.Visibility = tab == "log" ? Visibility.Visible : Visibility.Collapsed;

            TabChatBtn.Style = tab == "chat" ? (Style)FindResource("TabButtonActive") : (Style)FindResource("TabButtonInactive");
            TabTaskBtn.Style = tab == "task" ? (Style)FindResource("TabButtonActive") : (Style)FindResource("TabButtonInactive");
            TabQuizBtn.Style = tab == "quiz" ? (Style)FindResource("TabButtonActive") : (Style)FindResource("TabButtonInactive");
            TabLogBtn.Style = tab == "log" ? (Style)FindResource("TabButtonActive") : (Style)FindResource("TabButtonInactive");
        }

        // ══════════════════════════════════════════════════════════════════════
        // TASK ASSISTANT
        // ══════════════════════════════════════════════════════════════════════

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleInput.Text.Trim();
            string desc = TaskDescInput.Text.Trim();
            string reminder = TaskReminderInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                TaskStatusLabel.Text = "⚠️ Please enter a task title.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(248, 81, 73));
                return;
            }

            bool success = DatabaseHelper.AddTask(title, desc, reminder);
            if (success)
            {
                string reminderMsg = string.IsNullOrWhiteSpace(reminder) ? "no reminder" : $"reminder: {reminder}";
                TaskStatusLabel.Text = $"✅ Task saved: '{title}' ({reminderMsg})";
                TaskStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(63, 185, 80));
                ActivityLogger.Log($"Task added: '{title}' — {reminderMsg}.");
                TaskTitleInput.Clear();
                TaskDescInput.Clear();
                TaskReminderInput.Clear();
                RefreshTaskList();
            }
            else
            {
                TaskStatusLabel.Text = "❌ Failed to save task. Check DB connection.";
                TaskStatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(248, 81, 73));
            }
        }

        private void RefreshTasks_Click(object sender, RoutedEventArgs e) => RefreshTaskList();

        private void RefreshTaskList()
        {
            TaskListPanel.Children.Clear();
            var tasks = DatabaseHelper.GetAllTasks();

            if (tasks.Count == 0)
            {
                TaskListPanel.Children.Add(new TextBlock
                {
                    Text = "No tasks yet. Add a cybersecurity task using the form!",
                    Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                    FontSize = 13,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                return;
            }

            foreach (var task in tasks)
            {
                var card = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(22, 27, 34)),
                    BorderBrush = task.IsCompleted
                        ? new SolidColorBrush(Color.FromRgb(63, 185, 80))
                        : new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(14, 10, 14, 10),
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var cardGrid = new Grid();
                cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                cardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var info = new StackPanel();
                Grid.SetColumn(info, 0);

                info.Children.Add(new TextBlock
                {
                    Text = (task.IsCompleted ? "✅ " : "🔒 ") + task.Title,
                    Foreground = new SolidColorBrush(task.IsCompleted
                        ? Color.FromRgb(63, 185, 80)
                        : Color.FromRgb(230, 237, 243)),
                    FontSize = 14,
                    FontWeight = FontWeights.Bold
                });

                if (!string.IsNullOrWhiteSpace(task.Description))
                    info.Children.Add(new TextBlock
                    {
                        Text = task.Description,
                        Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                        FontSize = 12,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 2, 0, 0)
                    });

                if (!string.IsNullOrWhiteSpace(task.ReminderDate))
                    info.Children.Add(new TextBlock
                    {
                        Text = $"⏰ Reminder: {task.ReminderDate}",
                        Foreground = new SolidColorBrush(Color.FromRgb(255, 166, 0)),
                        FontSize = 11,
                        Margin = new Thickness(0, 2, 0, 0)
                    });

                info.Children.Add(new TextBlock
                {
                    Text = $"Added: {task.CreatedAt}",
                    Foreground = new SolidColorBrush(Color.FromRgb(72, 79, 88)),
                    FontSize = 10,
                    Margin = new Thickness(0, 2, 0, 0)
                });

                cardGrid.Children.Add(info);

                var btnPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(btnPanel, 1);

                if (!task.IsCompleted)
                {
                    int taskId = task.Id;
                    var completeBtn = new Button
                    {
                        Content = "✓ Done",
                        Style = (Style)FindResource("SuccessButton"),
                        Margin = new Thickness(8, 0, 0, 0)
                    };
                    completeBtn.Click += (s, ev) =>
                    {
                        DatabaseHelper.MarkTaskCompleted(taskId);
                        ActivityLogger.Log($"Task completed: '{task.Title}'.");
                        RefreshTaskList();
                    };
                    btnPanel.Children.Add(completeBtn);
                }

                int delId = task.Id;
                string delTitle = task.Title;
                var deleteBtn = new Button
                {
                    Content = "🗑 Delete",
                    Style = (Style)FindResource("DangerButton"),
                    Margin = new Thickness(6, 0, 0, 0)
                };
                deleteBtn.Click += (s, ev) =>
                {
                    DatabaseHelper.DeleteTask(delId);
                    ActivityLogger.Log($"Task deleted: '{delTitle}'.");
                    RefreshTaskList();
                };
                btnPanel.Children.Add(deleteBtn);
                cardGrid.Children.Add(btnPanel);

                card.Child = cardGrid;
                TaskListPanel.Children.Add(card);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // QUIZ GAME
        // ══════════════════════════════════════════════════════════════════════

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quizQuestions = QuizData.GetQuestions();
            var rng = new Random();
            for (int i = _quizQuestions.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (_quizQuestions[i], _quizQuestions[j]) = (_quizQuestions[j], _quizQuestions[i]);
            }
            _quizCurrentIndex = 0;
            _quizScore = 0;
            _quizAnswered = false;
            StartQuizBtn.Visibility = Visibility.Collapsed;
            NextQuestionBtn.Visibility = Visibility.Collapsed;
            ActivityLogger.Log("Quiz started.");
            DisplayCurrentQuestion();
        }

        private void DisplayCurrentQuestion()
        {
            if (_quizQuestions == null || _quizCurrentIndex >= _quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            _quizAnswered = false;
            QuizFeedbackBorder.Visibility = Visibility.Collapsed;
            NextQuestionBtn.Visibility = Visibility.Collapsed;

            var q = _quizQuestions[_quizCurrentIndex];
            QuizQuestionLabel.Text = $"Q{_quizCurrentIndex + 1}: {q.Question}";
            QuizProgressLabel.Text = $"{_quizCurrentIndex + 1} / {_quizQuestions.Count}";
            QuizScoreLabel.Text = _quizScore.ToString();
            QuizStatusBadge.Text = "In Progress";
            QuizStatusBadge.Foreground = new SolidColorBrush(Color.FromRgb(255, 166, 0));

            QuizOptionsPanel.Children.Clear();
            for (int i = 0; i < q.Options.Count; i++)
            {
                int answerIndex = i;
                var optBtn = new Button
                {
                    Content = q.Options[i],
                    Style = (Style)FindResource("ChipButton"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    FontSize = 13,
                    Padding = new Thickness(14, 10, 14, 10),
                    Margin = new Thickness(0, 0, 0, 6)
                };
                optBtn.Click += (s, ev) => HandleQuizAnswer(answerIndex);
                QuizOptionsPanel.Children.Add(optBtn);
            }
        }

        private void HandleQuizAnswer(int selectedIndex)
        {
            if (_quizAnswered || _quizQuestions == null) return;
            _quizAnswered = true;

            var q = _quizQuestions[_quizCurrentIndex];
            bool correct = selectedIndex == q.CorrectIndex;
            if (correct) _quizScore++;

            for (int i = 0; i < QuizOptionsPanel.Children.Count; i++)
            {
                if (QuizOptionsPanel.Children[i] is Button btn)
                {
                    btn.IsEnabled = false;
                    if (i == q.CorrectIndex)
                        btn.Background = new SolidColorBrush(Color.FromRgb(35, 134, 54));
                    else if (i == selectedIndex && !correct)
                        btn.Background = new SolidColorBrush(Color.FromRgb(218, 54, 51));
                }
            }

            string feedback = correct
                ? q.Explanation
                : $"❌ Incorrect! The correct answer was: {q.Options[q.CorrectIndex]}\n\n{q.Explanation}";

            QuizFeedbackLabel.Text = feedback;
            QuizFeedbackBorder.BorderBrush = correct
                ? new SolidColorBrush(Color.FromRgb(63, 185, 80))
                : new SolidColorBrush(Color.FromRgb(248, 81, 73));
            QuizFeedbackBorder.Visibility = Visibility.Visible;
            QuizScoreLabel.Text = _quizScore.ToString();

            ActivityLogger.Log($"Quiz Q{_quizCurrentIndex + 1} answered — {(correct ? "Correct" : "Incorrect")}.");

            if (_quizCurrentIndex < _quizQuestions.Count - 1)
                NextQuestionBtn.Visibility = Visibility.Visible;
            else
            {
                NextQuestionBtn.Content = "See Results 🏆";
                NextQuestionBtn.Visibility = Visibility.Visible;
            }
        }

        private void NextQuestion_Click(object sender, RoutedEventArgs e)
        {
            _quizCurrentIndex++;
            if (_quizQuestions == null || _quizCurrentIndex >= _quizQuestions.Count)
                EndQuiz();
            else
            {
                NextQuestionBtn.Content = "Next Question  ➤";
                DisplayCurrentQuestion();
            }
        }

        private void EndQuiz()
        {
            int total = _quizQuestions?.Count ?? 0;
            double pct = total > 0 ? (double)_quizScore / total * 100 : 0;

            string grade = pct >= 90 ? "🏆 Great job! You're a cybersecurity pro!" :
                           pct >= 70 ? "👍 Good work! Keep sharpening your skills." :
                           pct >= 50 ? "📚 Keep learning to stay safe online!" :
                                       "🔄 Practice makes perfect. Keep learning!";

            QuizQuestionLabel.Text = $"Quiz Complete! 🎉\n\nYour Score: {_quizScore} / {total} ({pct:0}%)\n\n{grade}";
            QuizOptionsPanel.Children.Clear();
            QuizFeedbackBorder.Visibility = Visibility.Collapsed;
            NextQuestionBtn.Visibility = Visibility.Collapsed;
            QuizStatusBadge.Text = "Done!";
            QuizStatusBadge.Foreground = new SolidColorBrush(Color.FromRgb(63, 185, 80));
            StartQuizBtn.Content = "🔄  Play Again";
            StartQuizBtn.Visibility = Visibility.Visible;
            QuizProgressLabel.Text = $"{total} / {total}";

            ActivityLogger.Log($"Quiz completed — Score: {_quizScore}/{total} ({pct:0}%).");
        }

        // ══════════════════════════════════════════════════════════════════════
        // ACTIVITY LOG
        // ══════════════════════════════════════════════════════════════════════

        private void RefreshActivityLog()
        {
            LogPanel.Children.Clear();
            var entries = ActivityLogger.GetRecent(_logShowCount);
            LogCountLabel.Text = $"{ActivityLogger.Count} actions recorded this session.";
            ShowMoreLogBtn.Visibility = ActivityLogger.Count > _logShowCount
                ? Visibility.Visible : Visibility.Collapsed;

            if (entries.Count == 0)
            {
                LogPanel.Children.Add(new TextBlock
                {
                    Text = "No activity yet. Start chatting, add tasks, or take the quiz!",
                    Foreground = new SolidColorBrush(Color.FromRgb(139, 148, 158)),
                    FontSize = 13
                });
                return;
            }

            for (int i = 0; i < entries.Count; i++)
            {
                var row = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(22, 27, 34)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(48, 54, 61)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(14, 8, 14, 8),
                    Margin = new Thickness(0, 0, 0, 6)
                };

                var rowGrid = new Grid();
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var numLabel = new TextBlock
                {
                    Text = $"{i + 1}.",
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                    FontWeight = FontWeights.Bold,
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(numLabel, 0);

                var entryLabel = new TextBlock
                {
                    Text = entries[i],
                    Foreground = new SolidColorBrush(Color.FromRgb(230, 237, 243)),
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(entryLabel, 1);

                rowGrid.Children.Add(numLabel);
                rowGrid.Children.Add(entryLabel);
                row.Child = rowGrid;
                LogPanel.Children.Add(row);
            }
        }

        private void ShowMoreLog_Click(object sender, RoutedEventArgs e)
        {
            _logShowCount += 5;
            RefreshActivityLog();
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            ActivityLogger.Clear();
            _logShowCount = 5;
            ActivityLogger.Log("Activity log cleared.");
            RefreshActivityLog();
        }

        // ══════════════════════════════════════════════════════════════════════
        // CHAT + NLP PROCESSING
        // ══════════════════════════════════════════════════════════════════════

        private void SendBtn_Click(object sender, RoutedEventArgs e) => ProcessInput();

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessInput();
        }

        private void Chip_Click(object sender, RoutedEventArgs e)
        {
            if (!_chatStarted) return;
            var btn = (Button)sender;
            UserInput.Text = btn.Tag.ToString();
            ProcessInput();
        }

        private void UserInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UserInput.Text))
                UpdateSentimentLabel(Responses.DetectSentiment(UserInput.Text));
        }

        private void ProcessInput()
        {
            if (!_chatStarted) return;
            string raw = UserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(raw)) return;
            UserInput.Clear();

            AddUserMessage(raw);
            _memory.ScanAndStore(raw);
            _memory.IncrementExchange();

            string input = raw.ToLower();
            string sentiment = Responses.DetectSentiment(raw);
            UpdateSentimentLabel(sentiment);

            ActivityLogger.Log($"User said: \"{raw}\"");

            // ── Pending reminder flow ──────────────────────────────────────────
            if (_awaitingReminder)
            {
                HandleReminderResponse(raw, input);
                return;
            }

            // ── NLP: Quiz ─────────────────────────────────────────────────────
            if (ContainsAny(input, "start quiz", "take quiz", "play quiz", "begin quiz",
                            "quiz me", "test me", "test my knowledge", "cybersecurity quiz"))
            {
                ShowTab("quiz");
                AddBotMessage("🎮 Opening the Quiz! Click 'Start Quiz' to begin.");
                ActivityLogger.Log("Navigated to Quiz via chat.");
                return;
            }

            // ── NLP: Activity Log ─────────────────────────────────────────────
            if (ContainsAny(input, "show log", "activity log", "what have you done",
                            "history", "recent actions", "show activity", "what happened"))
            {
                ShowTab("log");
                _logShowCount = 5;
                RefreshActivityLog();
                AddBotMessage("📋 Here's your Activity Log! Shows 5 at a time — click 'Show More' for older entries.");
                ActivityLogger.Log("Navigated to Activity Log via chat.");
                return;
            }

            // ── NLP: Add Task ─────────────────────────────────────────────────
            if (ContainsAny(input, "add task", "new task", "create task", "set task",
                            "remind me", "set reminder", "set a reminder", "add a reminder",
                            "can you remind", "i need to", "i want to", "help me remember",
                            "schedule", "don't forget", "remember to", "set up",
                            "enable 2fa", "update password", "review privacy", "check my"))
            {
                HandleNlpTaskRequest(raw, input);
                return;
            }

            // ── NLP: View Tasks ───────────────────────────────────────────────
            if (ContainsAny(input, "view tasks", "show tasks", "my tasks", "list tasks",
                            "what tasks", "open tasks", "manage tasks"))
            {
                ShowTab("task");
                RefreshTaskList();
                AddBotMessage("✅ Opening your Task Assistant! Here are your saved cybersecurity tasks.");
                ActivityLogger.Log("Navigated to Task Assistant via chat.");
                return;
            }

            // ── Follow-up ─────────────────────────────────────────────────────
            if (Responses.IsFollowUp(input) && _lastResponseTopic != null)
            {
                string followUpResponse = Responses.GetResponse(_lastResponseTopic, _userName, sentiment);
                AddBotMessage($"Here's more on {_lastResponseTopic} for you, {_userName}:\n\n{followUpResponse}");
                ActivityLogger.Log($"Follow-up on topic: {_lastResponseTopic}.");
                return;
            }

            // ── Standard response ─────────────────────────────────────────────
            string response = Responses.GetResponse(raw, _userName, sentiment);

            if (response == null)
            {
                AddBotMessage($"I didn't quite catch that, {_userName}. Try: 'add task', 'start quiz', 'show log', or a topic like 'phishing' or 'password'.");
                return;
            }

            if (response == "EXIT")
            {
                AddBotMessage($"Thanks for chatting, {_userName}! 🛡️\n\nYou covered {_memory.TopicCount} topic(s).\n\nStay safe online — Think Before You Click! 👋");
                UserInput.IsEnabled = false;
                SendBtn.IsEnabled = false;
                EnableChips(false);
                ActivityLogger.Log($"Chat session ended by {_userName}.");
                return;
            }

            if (response == "default")
            {
                string? lastTopic = _memory.LastTopic();
                string fallback = lastTopic != null
                    ? $"Hmm, not sure about that, {_userName}. We were discussing {lastTopic} — want more?\n\nOr try: 'add task', 'start quiz', 'show log'."
                    : $"I didn't understand that, {_userName}. Try: 'add task', 'start quiz', 'show log', or a cybersecurity topic!";
                AddBotMessage(fallback);
                return;
            }

            string sentimentPrefix = Responses.GetSentimentPrefix(sentiment, _userName);
            string memoryNote = _memory.GetContextNote(_userName);
            AddBotMessage(memoryNote + sentimentPrefix + response);

            _lastResponseTopic = _memory.LastTopic();
            ActivityLogger.Log($"Bot responded on topic: {_lastResponseTopic ?? "general"}.");

            string? milestone = _memory.GetMilestoneMessage(_userName);
            if (milestone != null) AddBotMessage(milestone, isTip: true);

            if (_memory.ExchangeCount > 0 && _memory.ExchangeCount % 4 == 0)
                AddBotMessage(Responses.GetRandomTip(), isTip: true);
        }

        // ── NLP Task Handler ────────────────────────────────────────────────────
        private void HandleNlpTaskRequest(string raw, string input)
        {
            string title = ExtractTaskTitle(raw, input);

            if (string.IsNullOrWhiteSpace(title))
            {
                AddBotMessage($"Sure, {_userName}! What is the task title?\n(e.g. 'Enable two-factor authentication', 'Review account privacy settings')");
                _awaitingReminder = false;
                ShowTab("task");
                return;
            }

            _pendingTaskTitle = title;
            _pendingTaskDesc = $"Cybersecurity task: {title}";
            _awaitingReminder = true;
            AddBotMessage($"Task identified: '{title}' ✅\n\nWould you like a reminder?\n• 'Yes, remind me in 3 days'\n• 'Remind me on 2026-07-15'\n• 'No reminder' to skip.");
            ActivityLogger.Log($"NLP task request: '{title}' — awaiting reminder.");
        }

        private void HandleReminderResponse(string raw, string input)
        {
            _awaitingReminder = false;
            string reminder = "";

            if (ContainsAny(input, "no", "skip", "none", "no reminder", "don't", "do not"))
                reminder = "";
            else
                reminder = raw;

            bool success = DatabaseHelper.AddTask(_pendingTaskTitle, _pendingTaskDesc, reminder);
            if (success)
            {
                string reminderText = string.IsNullOrWhiteSpace(reminder) ? "no reminder set" : $"reminder: {reminder}";
                AddBotMessage($"✅ Task saved: '{_pendingTaskTitle}'\n({reminderText})\n\nView it in the ✅ Task Assistant tab!");
                ActivityLogger.Log($"Task saved via NLP: '{_pendingTaskTitle}' — {reminderText}.");
                RefreshTaskList();
            }
            else
            {
                AddBotMessage("❌ Couldn't save the task. Check your DB connection and try the Task Assistant tab.");
            }
            _pendingTaskTitle = "";
            _pendingTaskDesc = "";
        }

        // ── Helpers ─────────────────────────────────────────────────────────────
        private static bool ContainsAny(string input, params string[] keywords)
        {
            foreach (var kw in keywords)
                if (input.Contains(kw, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static string ExtractTaskTitle(string raw, string input)
        {
            var patterns = new[]
            {
                "add task - ", "add task: ", "add task ", "create task: ", "new task: ",
                "remind me to ", "remind me about ", "set a reminder to ", "set a reminder for ",
                "i need to ", "i want to ", "help me remember to ",
                "add a reminder to ", "add a reminder for ", "don't forget to ", "remember to "
            };

            foreach (var p in patterns)
            {
                int idx = input.IndexOf(p, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    string extracted = raw.Substring(idx + p.Length).Trim();
                    if (extracted.Length > 2)
                        return char.ToUpper(extracted[0]) + extracted.Substring(1);
                }
            }

            if (ContainsAny(input, "enable 2fa", "two-factor", "two factor")) return "Enable Two-Factor Authentication";
            if (ContainsAny(input, "update password", "change password")) return "Update Password";
            if (ContainsAny(input, "review privacy", "privacy settings")) return "Review Account Privacy Settings";
            if (ContainsAny(input, "backup", "back up")) return "Create a Data Backup";
            if (ContainsAny(input, "antivirus", "anti-virus")) return "Install/Update Antivirus Software";
            if (ContainsAny(input, "vpn")) return "Set Up VPN";

            return "";
        }

        // ── UI Helpers ──────────────────────────────────────────────────────────
        private void AddBotMessage(string text, bool isTip = false, bool isGreeting = false)
        {
            var row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 3, 0, 3)
            };

            var avatar = new Border
            {
                Width = 34,
                Height = 34,
                Background = isTip
                    ? new SolidColorBrush(Color.FromRgb(139, 92, 246))
                    : new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                CornerRadius = new CornerRadius(17),
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            avatar.Child = new TextBlock
            {
                Text = isTip ? "💡" : "🤖",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            row.Children.Add(avatar);

            var bubble = new Border
            {
                Background = isTip
                    ? new SolidColorBrush(Color.FromRgb(22, 15, 35))
                    : new SolidColorBrush(Color.FromRgb(22, 27, 34)),
                BorderBrush = isTip
                    ? new SolidColorBrush(Color.FromRgb(139, 92, 246))
                    : new SolidColorBrush(Color.FromRgb(0, 212, 255)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(0, 12, 12, 12),
                Padding = new Thickness(14, 10, 14, 10),
                MaxWidth = 650
            };
            bubble.Child = new TextBlock
            {
                Text = text,
                Foreground = new SolidColorBrush(Color.FromRgb(230, 237, 243)),
                FontSize = isGreeting ? 14 : 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };
            row.Children.Add(bubble);
            ChatPanel.Children.Add(row);
            AnimateFadeIn(row);
            ScrollToBottom();
        }

        private void AddUserMessage(string text)
        {
            var row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 3, 0, 3)
            };
            var bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(31, 111, 235)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(56, 139, 253)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(12, 0, 12, 12),
                Padding = new Thickness(14, 10, 14, 10),
                MaxWidth = 550
            };
            bubble.Child = new TextBlock
            {
                Text = $"👤  {_userName}:  {text}",
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 22
            };
            row.Children.Add(bubble);
            ChatPanel.Children.Add(row);
            AnimateFadeIn(row);
            ScrollToBottom();
        }

        private static void AnimateFadeIn(UIElement element)
        {
            element.Opacity = 0;
            var anim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            element.BeginAnimation(OpacityProperty, anim);
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.UpdateLayout();
            ChatScrollViewer.ScrollToBottom();
        }

        private void UpdateSentimentLabel(string sentiment)
        {
            var (emoji, label, color) = sentiment switch
            {
                "worried" => ("😟", "Worried", Color.FromRgb(248, 81, 73)),
                "frustrated" => ("😤", "Frustrated", Color.FromRgb(255, 166, 0)),
                "curious" => ("🤔", "Curious", Color.FromRgb(88, 230, 255)),
                "happy" => ("😊", "Happy", Color.FromRgb(63, 185, 80)),
                _ => ("😐", "Neutral", Color.FromRgb(139, 148, 158))
            };
            SentimentLabel.Text = $"{emoji}  {label}";
            SentimentLabel.Foreground = new SolidColorBrush(color);
        }

        private void EnableChips(bool enabled)
        {
            foreach (UIElement child in ChipsPanel.Children)
                if (child is Button btn) btn.IsEnabled = enabled;
        }
    }
}