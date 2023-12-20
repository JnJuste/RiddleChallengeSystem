using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;

namespace RiddleChallengeSystem.Pages
{
    public class RiddleManagementModel : PageModel
    {
        String conString = "Data Source=JN-JUSTE\\SQLEXPRESS;Initial Catalog=RiddlesChallengeSystemDB;Integrated Security=True;Encrypt=False";

        public CreateRiddle createRiddle = new CreateRiddle();

        public List<CreateRiddle> riddleList = new List<CreateRiddle>();

        public string message = "";
        public List<SelectListItem> DifficultyLevels { get; set; }
        public List<SelectListItem> Categories { get; set; }

        
        public void OnGet()
        {

        }

        public void OnPost() 
        {
            try
            {
                createRiddle.Question = Request.Form["question"];
                createRiddle.Answer = Request.Form["answer"];

            }
            catch (Exception ex)
            {
                message = "There's a problem: " + ex.Message;
            }
            using (SqlConnection con = new SqlConnection(conString))
            {
                string qry = "INSERT INTO RiddleTable VALUES (@question, @answer)";

                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(qry, con))
                    {
                        cmd.Parameters.AddWithValue("@question", createRiddle.Question);
                        cmd.Parameters.AddWithValue("@answer", createRiddle.Answer);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Riddle Added Successfully";
                            createRiddle = new CreateRiddle(); // empty the inputs
                        }
                        else
                        {
                            message = "Riddle Not Added Successfully";
                        }
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                       message = "There's a problem: " + ex.Message;
                }
            }

        }
    }
}
