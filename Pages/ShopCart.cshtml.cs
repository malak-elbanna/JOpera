using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
namespace Project_test.Pages
{
    public class ShopCartModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Dictionary<int, List<int>> CartDictionary { get; set; }
        public void OnGet(int UID)
        {
            // Check if UID exists in the dictionary
            if (CartDictionary.ContainsKey(UID))
            {
                foreach (var PID in CartDictionary[UID])
                {
                    string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

                    string query = $"SELECT Name, Price FROM Product WHERE ProductID = {PID}";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                //function here for displaying on the page.
                            }
                        }
                    }
                }
            }
        }
        public IActionResult OnGetProduct(int UID, int PID)
        {
            if (!CartDictionary.ContainsKey(UID))
            {
                CartDictionary[UID] = new List<int>();
            }

            if (!CartDictionary[UID].Contains(PID))
            {
                CartDictionary[UID].Add(PID);
            }
            return RedirectToPage("/ProductView", new { UID });
        }
    }
}
