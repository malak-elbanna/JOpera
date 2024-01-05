using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace Project_test.Pages
{
    public class ServicesModel : PageModel
    {
        public int? userId { get; set; }

        public List<ServicesInfo> Services = new List<ServicesInfo>();
        private readonly HashSet<int> displayedServiceIDs = new HashSet<int>();
        private readonly ILogger<ProductsModel> _logger;

        public ServicesModel(ILogger<ProductsModel> logger)
        {

            _logger = logger;
        }
        public void OnGet(string category)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            try
            {
                //string connectionString = "Data Source=DESKTOP-05RUH8H;Initial Catalog=joperaffff;Integrated Security=True";
                string connectionString = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";
                //string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
                //string connectionString = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT S.[Name], S.[Price], S.[Category], SI.[img], S.[ServiceID], S.FreelancerID FROM Service AS S\r\nJOIN ServiceIMG AS SI ON S.ServiceID = SI.ServiceID";

                    if (!string.IsNullOrEmpty(category))
                    {
                        query += $" WHERE S.[Category] = @category";
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
                                int ServiceID = data.GetInt32(data.GetOrdinal("ServiceID"));
                                if (!displayedServiceIDs.Contains(ServiceID)) {
                                    displayedServiceIDs.Add(ServiceID);
                                    HttpContext.Session.SetInt32("ServiceID", ServiceID);
                                    ServicesInfo info = new ServicesInfo();
                                    info.Price = "" + data.GetInt32(1);
                                    info.Name = data.GetString(0);
                                    info.Category = data.GetString(2);
                                    info.ImageData = data.GetSqlBinary(3).Value;
                                    info.serviceID = data.GetInt32(4);
                                    info.FreelancerID = data.GetInt32(5);

                                    Services.Add(info);
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

        public class ServicesInfo
        {
            public string Name;
            public string Price;
            public int serviceID;
            public int FreelancerID;
            public string Category { get; set; }

            public byte[] ImageData { get; set; }

        }
    }
}
