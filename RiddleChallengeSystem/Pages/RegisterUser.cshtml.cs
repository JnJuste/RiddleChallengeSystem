using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace RiddleChallengeSystem.Pages
{
    public class RegisterUserModel : PageModel
    {
        private readonly string conString = "Data Source=JN-JUSTE\\SQLEXPRESS;Initial Catalog=RiddlesChallengeSystemDB;Integrated Security=True;Encrypt=False";

        [BindProperty]
        public string Message { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            // Retrieve form data
            string userName = Request.Form["UserNames"];
            string email = Request.Form["Email"];
            string password = Request.Form["Password"];
            string role = Request.Form["Role"];

            // Validate and save to database
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        con.Open();
                        string qry = "INSERT INTO UserTable (UserNames, Email, Password, Role) VALUES (@userName, @email, @password, @role)";
                        using (SqlCommand cmd = new SqlCommand(qry, con))
                        {
                            cmd.Parameters.AddWithValue("@userName", userName);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                            cmd.Parameters.AddWithValue("@role", role);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Message = "Registration successful! You can login";
                            }
                            else
                            {
                                Message = "Registration failed. Please Try Again!";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Message = "There's a problem: " + ex.Message;
                }
            }
            else
            {
                Message = "Please fill in all the fields.";
            }

            return Page();
        }
    }
}
