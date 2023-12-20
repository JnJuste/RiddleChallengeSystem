namespace RiddleChallengeSystem.Pages
{
    public class UserRiddleAttempt
    {
        public int AttemptID { get; set; }
        public int UserID { get; set; }
        public int RiddleID { get; set; }
        public string UserAnswer { get; set; }
        public bool IsCorrect { get; set; } = true;
        public int Score { get; set;}

        public UserRiddleAttempt() { }
        public UserRiddleAttempt(int attemptID, int userID, int riddleID, string userAnswer, bool isCorrect, int score)
        {
            AttemptID = attemptID;
            UserID = userID;
            RiddleID = riddleID;
            UserAnswer = userAnswer;
            IsCorrect = isCorrect;
            Score = score;
        }
    }
}
