using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace RiddleChallengeSystem.Pages
{
    public class EditRiddleModel : PageModel
    {
        public RiddleTableInfo riddle = new RiddleTableInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
            if (!int.TryParse(Request.Query["RiddleID"], out int riddleID))
            {
                errorMessage = "Invalid or missing RiddleID parameter.";
                return;
            }

            try
            {
                string conString = "Data Source=JN-JUSTE\\SQLEXPRESS;Initial Catalog=RiddlesChallengeSystemDB;Integrated Security=True;Encrypt=False";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM RiddleTable WHERE RiddleID = @RiddleID";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@RiddleID", riddleID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                riddle.RiddleID = reader.GetInt32(0);
                                riddle.Question = reader.GetString(1);
                                riddle.Answer = reader.GetString(2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }

        public void OnPost()
        {
            if (!int.TryParse(Request.Form["RiddleID"], out int riddleID))
            {
                errorMessage = "Invalid or missing RiddleID parameter.";
                return;
            }

            riddle.RiddleID = riddleID;
            riddle.Question = Request.Form["Question"];
            riddle.Answer = Request.Form["Answer"];
            
            if (string.IsNullOrEmpty(riddle.Answer) || string.IsNullOrEmpty(riddle.Question))
            {
                errorMessage = "Answer and Question of the riddle are required";
                return;
            }

            try
            {
                string conString = "Data Source=JN-JUSTE\\SQLEXPRESS;Initial Catalog=RiddlesChallengeSystemDB;Integrated Security=True;Encrypt=False";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    string sqlQuery = "UPDATE RiddleTable SET Answer = @Answer, Question = @Question WHERE RiddleID = @RiddleID";
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@RiddleID", riddleID);
                        cmd.Parameters.AddWithValue("@Answer", riddle.Answer);
                        cmd.Parameters.AddWithValue("@Question", riddle.Question);
                      

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            riddle.Question = "";
            riddle.Answer = "";

            successMessage = "Riddle Info Updated with success";
            Response.Redirect("/ViewRiddles"); 
        }
    }

    
}
