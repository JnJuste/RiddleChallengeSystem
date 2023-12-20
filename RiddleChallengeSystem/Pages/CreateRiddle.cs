namespace RiddleChallengeSystem.Pages
{
    public class CreateRiddle
    {
        public int RiddleID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        

        public CreateRiddle() { }

        public CreateRiddle(int riddleID, string question, string answer)
        {
            RiddleID = riddleID;
            Question = question;
            Answer = answer;
        }
    }
}
