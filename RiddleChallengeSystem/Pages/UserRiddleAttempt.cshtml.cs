using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace RiddleChallengeSystem.Pages
{
    public class UserRiddleAttemptModel : PageModel
    {
        private readonly string conString = "Data Source=JN-JUSTE\\SQLEXPRESS;Initial Catalog=RiddlesChallengeSystemDB;Integrated Security=True;Encrypt=False";

        public UserRiddleAttempt userRiddleAtt = new UserRiddleAttempt();
        public List<UserRiddleAttempt> userRiddleList = new List<UserRiddleAttempt>();
        public List<SelectListItem> UserNameList { get; set; }
        public List<SelectListItem> RiddleList { get; set; }

        public string Message = "";

        public void OnGet()
        {
            UserNameList = GetUserNameList();
            RiddleList = GetRiddleList();
        }

        private List<SelectListItem> GetUserNameList()
        {
            List<SelectListItem> userList = new List<SelectListItem>();

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT UserID, UserNames FROM UserTable";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int userID = reader.GetInt32(0);
                                string userNames = reader.GetString(1);
                                userList.Add(new SelectListItem { Value = userID.ToString(), Text = userNames });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return userList;
        }

        private List<SelectListItem> GetRiddleList()
        {
            List<SelectListItem> riddleList = new List<SelectListItem>();

            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT RiddleID, Question FROM RiddleTable";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int riddleID = reader.GetInt32(0);
                                string question = reader.GetString(1);
                                riddleList.Add(new SelectListItem { Value = riddleID.ToString(), Text = question });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return riddleList;
        }

        public void OnPost()
        {
            try
            {
                string correctAnswer = GetCorrectAnswer(userRiddleAtt.RiddleID);
                userRiddleAtt.IsCorrect = string.Equals(userRiddleAtt.UserAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);
                userRiddleAtt.Score = CalculateScore(userRiddleAtt);

                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    string insertQuery = "INSERT INTO UserRiddleAttempt (UserID, RiddleID, UserAnswer, IsCorrect, Score) " +
                                         "VALUES (@UserID, @RiddleID, @UserAnswer, @IsCorrect, @Score)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userRiddleAtt.UserID);
                        cmd.Parameters.AddWithValue("@RiddleID", userRiddleAtt.RiddleID);
                        cmd.Parameters.AddWithValue("@UserAnswer", userRiddleAtt.UserAnswer);
                        cmd.Parameters.AddWithValue("@IsCorrect", userRiddleAtt.IsCorrect);
                        cmd.Parameters.AddWithValue("@Score", userRiddleAtt.Score);

                        cmd.ExecuteNonQuery();
                    }
                }

                Message = $"Your answer is {(userRiddleAtt.IsCorrect ? "correct" : "incorrect")}. Your score is {userRiddleAtt.Score}.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Message = "An error occurred while processing your answer.";
            }
        }

        private string GetCorrectAnswer(int riddleID)
        {
            // Retrieve the correct answer from the RiddleTable based on the selected riddleID
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string sqlQuery = "SELECT Answer FROM RiddleTable WHERE RiddleID = @RiddleID";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    cmd.Parameters.AddWithValue("@RiddleID", riddleID);

                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : null;
                }
            }
        }


        private int CalculateScore(UserRiddleAttempt userRiddleAttempt)
        {
         
            return userRiddleAttempt.IsCorrect ? 10 : 0;
        }
    }
}
