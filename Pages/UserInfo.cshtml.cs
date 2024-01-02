using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_test.Pages
{
    public class FreeLancer_infoModel : PageModel
    {
        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == 0)
            {
                Console.WriteLine("LOGGED OUT");
            }
            else
            {
                Console.WriteLine("LOGGED IN WITH ID:");
                Console.WriteLine(userId);
            }
        }
    }
}
