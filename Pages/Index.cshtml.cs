using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_test.Models;

namespace Project_test.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public DB db { get; set; }

        public IndexModel(ILogger<IndexModel> logger, DB db)
        {
            db = db;
            _logger = logger;
        }

        public void OnGet()
        {
            //db.getUsers();
        }
    }
}