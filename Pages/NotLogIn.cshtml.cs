using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_test.Pages
{
    public class NotLogInModel : PageModel
    {
        public int? userId { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

        }
        public IActionResult OnPost()
        {
            return RedirectToPage("/Login");
        }
    }
}
