using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_test.Pages
{
    public class FreeLancer_infoModel : PageModel
    {
        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userId == 0)
            {
                Console.WriteLine("LOGGED OUT");
            }
            else
            {
                Console.WriteLine("LOGGED IN WITH ID:");
                Console.WriteLine(userId);
                Console.WriteLine("Role :");
                Console.WriteLine(userRole);
            }
        }
    }
}
