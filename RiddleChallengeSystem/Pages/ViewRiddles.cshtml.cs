using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace RiddleChallengeSystem.Pages
{
    public class ViewRiddlesModel : PageModel
    {
        public List<RiddleTableInfo> listRiddles = new List<RiddleTableInfo>();

        public void OnGet()
        {
            listRiddles.Clear();
            try
            {
                String conString = "Data Source=JN-JUSTE\\SQLEXPRESS;Initial Catalog=RiddlesChallengeSystemDB;Integrated Security=True;Encrypt=False";
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();
                    String sqlQuery = "SELECT * FROM RiddleTable"; // Updated query to retrieve users
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RiddleTableInfo user = new RiddleTableInfo();
                                user.RiddleID = reader.GetInt32(0); // Assuming UserID is the first column
                                user.Question = reader.GetString(1);
                                user.Answer = reader.GetString(2);
                                
                                listRiddles.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
            }
        }
    }

    public class RiddleTableInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RiddleID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
