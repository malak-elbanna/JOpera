using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Project_test.Pages
{
    public class ChangePassModel : PageModel
    {
        [BindProperty]
        public string? NewPassword { get; set; }
        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public IActionResult OnPostPassword()
        {
            string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection con = new SqlConnection(conStr))
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                string updateQuery = "UPDATE Users SET Password = @Password WHERE UserID = @UserId";

                try
                {
                    con.Open();
                    if (NewPassword == ConfirmPassword)
                    {
                        using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@Password", NewPassword);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                return RedirectToPage("/ThankYou");
                            }
                            return RedirectToPage("/UserInfo");
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Passwords do not match.";
                        return RedirectToPage("/ChangePass");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["ErrorMessage"] = "An error occurred while updating the password";
                    return RedirectToPage("/UserInfo");
                }
            }
        }
    }
}