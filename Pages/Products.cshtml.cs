using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;

namespace Project_test.Pages
{
    public class ProductsModel : PageModel
    {
        public List<Productssinfo> products = new List<Productssinfo>();
        private readonly ILogger<ProductsModel> _logger;

        public ProductsModel(ILogger<ProductsModel> logger)
        {

            _logger = logger;
        }
        public void OnGet(string category)
        {
            try
            {
                string connectionString = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT [Name],[Price],[Category] FROM Product";

                    if (!string.IsNullOrEmpty(category))
                    {
                        query += $" WHERE Category = @category";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(category))
                        {
                            command.Parameters.AddWithValue("@category", category);
                        }

                        using (SqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                Productssinfo info = new Productssinfo();
                                info.Price = "" + data.GetInt32(1);
                                info.Name = data.GetString(0);
                                info.Category = data.GetString(2);
                                products.Add(info);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public class Productssinfo
        {
            public string Name;
            public string Price;
            public string Category { get; set; }
        }
    }
}
