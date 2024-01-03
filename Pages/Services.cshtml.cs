using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace Project_test.Pages
{
    public class ServicesModel : PageModel
    {
        public List<ServicesInfo> Services = new List<ServicesInfo>();
        private readonly ILogger<ProductsModel> _logger;

        public ServicesModel(ILogger<ProductsModel> logger)
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
                    string query = "SELECT [Name],[Price],[Category] FROM Service";

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
                                ServicesInfo info = new ServicesInfo();
                                info.Price = "" + data.GetInt32(1);
                                info.Name = data.GetString(0);
                                info.Category = data.GetString(2);
                                Services.Add(info);
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

        public class ServicesInfo
        {
            public string Name;
            public string Price;
            public string Category { get; set; }
        }
    }
}
