using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_test.Pages.Shared
{
    public class _LayoutModel : PageModel
    {
        [BindProperty]
        public int? UserId { get; set; }

        public void OnGet()
        {
            UserId = HttpContext.Session.GetInt32("UserId");
        }
    }
}
