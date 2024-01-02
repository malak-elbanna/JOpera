using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_test.Pages
{
    public class FreeLancer_infoModel : PageModel
    {
        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine(userId);
        }
    }
}
