using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;

namespace Project_test.Pages
{
    public class ProductsModel : PageModel
    {
        public List<Productssinfo> products = new List<Productssinfo>();
        private readonly HashSet<int> displayedProductIDs = new HashSet<int>();
        private readonly ILogger<ProductsModel> _logger;

        public ProductsModel(ILogger<ProductsModel> logger)
        {

            _logger = logger;
        }
        public void OnGet(string category)
        {
            try
            {
                //string connectionString = "Data Source=DESKTOP-05RUH8H;Initial Catalog=JOperaF;Integrated Security=True";
                string connectionString = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
                //string connectionString = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                //string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //string query = "SELECT P.[Name], P.[Price], P.[Category], PI.[img] FROM Product AS P"+
                    //               "JOIN ProductIMG AS PI ON P.ProductID = PI.ProductID";

                    string query = "SELECT P.ProductID, P.[Name], P.[Price], P.[Category], PI.[img] FROM Product AS P JOIN ProductIMG AS PI ON P.ProductID = PI.ProductID";

                    if (!string.IsNullOrEmpty(category))
                    {
                        query += $" WHERE P.[Category] = @category";
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
                                int productID = data.GetInt32(data.GetOrdinal("ProductID"));
                                if (!displayedProductIDs.Contains(productID))
                                {
                                    displayedProductIDs.Add(productID);
                                    HttpContext.Session.SetInt32("ProductID", productID);
                                    Productssinfo info = new Productssinfo();
                                    info.Price = "" + data.GetInt32(2);
                                    info.Name = data.GetString(1);
                                    info.Category = data.GetString(3);
                                    info.ImageData = data.GetSqlBinary(4).Value;

                                    products.Add(info);
                                }
                               
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

            public byte[] ImageData { get; set; }

        
        }
    }
}
