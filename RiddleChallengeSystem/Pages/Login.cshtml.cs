using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RiddleChallengeSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly string conString = "Data Source=JN-JUSTE\\SQLEXPRESS;Initial Catalog=RiddlesChallengeSystemDB;Integrated Security=True;Encrypt=False";

        [BindProperty]
        public string Message { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            // Retrieve form data
            string email = Request.Form["Email"];
            string password = Request.Form["Password"];

            // Validate credentials
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        con.Open();
                        string qry = "SELECT * FROM UserTable WHERE Email = @email";
                        using (SqlCommand cmd = new SqlCommand(qry, con))
                        {
                            cmd.Parameters.AddWithValue("@email", email);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string role = reader["Role"].ToString();

                                    // Verify the password using BCrypt
                                    string hashedPassword = reader["Password"].ToString();
                                    if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                                    {
                                        var claims = new List<Claim>
                                        {
                                            new Claim(ClaimTypes.Name, email),
                                            new Claim(ClaimTypes.Role, role),
                                            new Claim(ClaimTypes.NameIdentifier, reader["UserID"].ToString())
                                        };

                                        var claimsIdentity = new ClaimsIdentity(
                                            claims, "CookieAuthenticationDefaults");

                                        var authProperties = new AuthenticationProperties
                                        {
                                            IsPersistent = true,
                                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                                        };

                                        await HttpContext.SignInAsync(
                                            "CookieAuthenticationDefaults",
                                            new ClaimsPrincipal(claimsIdentity),
                                            authProperties);

                                        if (role.Equals("RIDDLER", StringComparison.OrdinalIgnoreCase))
                                        {
                                            return RedirectToPage("/RiddleManagement"); // Replace with your admin page
                                        }
                                        else if (role.Equals("PLAYER", StringComparison.OrdinalIgnoreCase))
                                        {
                                            return RedirectToPage("/UserRiddleAttempt"); // Replace with your user page
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Message = "Invalid email or password.";
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
