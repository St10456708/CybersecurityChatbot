using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    /// <summary>
    /// Contains all cybersecurity quiz questions.
    /// Mix of multiple-choice and true/false — more than 10 questions.
    /// </summary>
    public static class QuizData
    {
        public class QuizQuestion
        {
            public string Question { get; set; } = "";
            public List<string> Options { get; set; } = new();
            public int CorrectIndex { get; set; }
            public string Explanation { get; set; } = "";
            public bool IsTrueFalse { get; set; }
        }

        public static List<QuizQuestion> GetQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "✅ Correct! Reporting phishing emails helps prevent scams and protects others.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "TRUE or FALSE: Using the same password for all accounts is safe.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "✅ False! Reusing passwords means one breach exposes ALL your accounts. Use a unique password per site.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "What does 2FA stand for?",
                    Options = new List<string> { "A) Two-Factor Authentication", "B) Two-Firewall Access", "C) Twice-Fixed Algorithm", "D) Transfer File Access" },
                    CorrectIndex = 0,
                    Explanation = "✅ Correct! Two-Factor Authentication adds a second layer of security beyond your password.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "Which of the following is the strongest password?",
                    Options = new List<string> { "A) password123", "B) John1990", "C) P@ssw0rd!", "D) Xk#9mQ!2vLp$7" },
                    CorrectIndex = 3,
                    Explanation = "✅ Correct! Long, random passwords mixing uppercase, lowercase, numbers, and symbols are strongest.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "TRUE or FALSE: Public Wi-Fi is safe for online banking.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "✅ False! Public Wi-Fi is unsecured. Hackers can intercept your data. Always use a VPN on public networks.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new List<string> { "A) Software that speeds up your PC", "B) Malware that encrypts your files and demands payment", "C) A type of antivirus", "D) A firewall tool" },
                    CorrectIndex = 1,
                    Explanation = "✅ Correct! Ransomware locks your files and demands money. Always keep backups!",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "What is social engineering in cybersecurity?",
                    Options = new List<string> { "A) Building social media platforms", "B) Manipulating people into revealing confidential info", "C) Engineering social apps", "D) A type of firewall" },
                    CorrectIndex = 1,
                    Explanation = "✅ Correct! Social engineering exploits human psychology, not technical vulnerabilities.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "TRUE or FALSE: HTTPS websites are completely secure and cannot be hacked.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "✅ False! HTTPS only encrypts data in transit. The website itself can still be compromised.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "What is a VPN used for?",
                    Options = new List<string> { "A) Increasing internet speed", "B) Encrypting your internet connection and hiding your IP", "C) Blocking ads", "D) Storing passwords" },
                    CorrectIndex = 1,
                    Explanation = "✅ Correct! A VPN encrypts your traffic and masks your IP address for privacy.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "Which is a sign of a phishing email?",
                    Options = new List<string> { "A) Sent from your known bank's official domain", "B) Contains your full name correctly", "C) Urgent language + suspicious link", "D) Has no attachments" },
                    CorrectIndex = 2,
                    Explanation = "✅ Correct! Phishing emails create urgency and contain suspicious links or attachments.",
                    IsTrueFalse = false
                },
                new QuizQuestion
                {
                    Question = "TRUE or FALSE: Antivirus software alone is enough to protect your computer.",
                    Options = new List<string> { "A) True", "B) False" },
                    CorrectIndex = 1,
                    Explanation = "✅ False! Antivirus is one layer. You also need updates, strong passwords, 2FA, and safe browsing habits.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "What should you do before clicking a link in an email?",
                    Options = new List<string> { "A) Click immediately if it looks official", "B) Hover over the link to verify the actual URL", "C) Forward it to friends", "D) Reply asking if it's real" },
                    CorrectIndex = 1,
                    Explanation = "✅ Correct! Always hover over links to see the real destination URL before clicking.",
                    IsTrueFalse = false
                }
            };
        }
    }
}
