using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Project_test.Pages
{
    public class OfferedSPModel : PageModel
    {
        public List<ProductServiceInfo> ProductsServices = new List<ProductServiceInfo>();
        private readonly HashSet<string> displayedServiceProductsIDs = new HashSet<string>();
        private readonly ILogger<OfferedSPModel> _logger;

        public OfferedSPModel(ILogger<OfferedSPModel> logger)
        {
            _logger = logger;
        }
        public int? userId { get; set; }

        public void OnGet(string type)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            try
            {
                string connectionString = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";
                //string connectionString = "Data Source = Bayoumi; Initial Catalog = JOpera; Integrated Security = True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //string query = "SELECT S.ServiceID, S.[Name], S.[Price], SI.[img] ,P.ProductID, P.[Name], P.[Price], PI.[img] FROM Service AS S, Product AS P JOIN ProductIMG AS PI ON P.ProductID = PI.ProductID WHERE P.FreelancerID = @userId";
                    string query = @"
    SELECT 
        'Product' AS Type, 
        P.ProductID AS ID, 
        P.[Name], 
        P.[Price], 
        PI.[img] AS ImageData
    FROM 
        Product AS P
    JOIN 
        ProductIMG AS PI ON P.ProductID = PI.ProductID
    WHERE 
        P.FreelancerID = @freelancerID

    UNION

    SELECT 
        'services' AS Type, 
        S.ServiceID AS ID, 
        S.[Name], 
        S.[Price], 
        SI.[img] AS ImageData
    FROM 
        Service AS S
    JOIN 
        ServiceIMG AS SI ON S.ServiceID = SI.ServiceID
    WHERE 
        S.FreelancerID = @freelancerID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@freelancerID", userId);

                        using (SqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                string entryKey = $"{data.GetString(0)}_{data.GetInt32(1)}";
                                if (!displayedServiceProductsIDs.Contains(entryKey))
                                {
                                    displayedServiceProductsIDs.Add(entryKey);
                                    ProductServiceInfo info = new ProductServiceInfo();
                                    info.Price = "" + data.GetInt32(3);
                                    info.Name = data.GetString(2);
                                    info.Type = data.GetString(0);
                                    info.ImageData = data.GetSqlBinary(4).Value;
                                    info.id = data.GetInt32(1);
                                    ProductsServices.Add(info);
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

        public class ProductServiceInfo
        {
            public string Name { get; set; }
            public string Price { get; set; }
            public byte[] ImageData { get; set; }
            public string Type { get; set; }
            public int id { get; set; }
        }
    }

}

