using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_test.Models;

namespace Project_test.Pages
{
    public class LogoutModel : PageModel
    {
        public int? userId { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

        }
        public IActionResult OnPost() {
            HttpContext.Session.SetInt32("UserId", 0);
            return RedirectToPage("/Login");
        }
    }
}
