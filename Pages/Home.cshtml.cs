using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Project_test.Pages
{
    public class HomeModel : PageModel
    {
        public int? userId { get; set; }
        private readonly ILogger<HomeModel> _logger;

        public List<Productssinfo> Products = new List<Productssinfo>();
        public List<ServicesInfo> Services = new List<ServicesInfo>();
        string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

        public HomeModel(ILogger<HomeModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                LoadProducts();
                LoadServices();
            }
            else
            {
                Console.WriteLine($"The string is not null and not empty and its value is: {keyword}.");
                LoadProducts(keyword);
                LoadServices(keyword);
            }
            var userId = HttpContext.Session.GetInt32("UserId");

        }

        private void LoadProducts(string keyword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT P.ProductID, P.[Name], P.[Price], P.[Category], PI.[img], P.FreelancerID " +
                                   "FROM Product AS P JOIN ProductIMG AS PI ON P.ProductID = PI.ProductID " +
                                   "WHERE P.[Name] LIKE @keyword OR P.[Category] LIKE @keyword";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                        using (SqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                Products.Add(new Productssinfo
                                {
                                    Price = data.GetInt32(2).ToString(),
                                    Name = data.GetString(1),
                                    Category = data.GetString(3),
                                    ImageData = data.GetSqlBinary(4).Value,
                                    ProductID = data.GetInt32(0),
                                    FreelancerID = data.GetInt32(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading products from the database");
            }
        }

        private void LoadServices(string keyword)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT S.[Name], S.[Price], S.[Category], SI.[img], S.[ServiceID] " +
                                   "FROM Service AS S JOIN ServiceIMG AS SI ON S.ServiceID = SI.ServiceID " +
                                   "WHERE S.[Name] LIKE @keyword OR S.[Category] LIKE @keyword";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@keyword", $"%{keyword}%");

                        using (SqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                Services.Add(new ServicesInfo
                                {
                                    Price = data.GetInt32(1).ToString(),
                                    Name = data.GetString(0),
                                    Category = data.GetString(2),
                                    ImageData = data.GetSqlBinary(3).Value,
                                    serviceID = data.GetInt32(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading services from the database");
            }

        }

        private void LoadProducts()
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT P.ProductID, P.[Name], P.[Price], P.[Category], PI.[img], P.FreelancerID FROM Product AS P JOIN ProductIMG AS PI ON P.ProductID = PI.ProductID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                int productID = data.GetInt32(data.GetOrdinal("ProductID"));
                                Productssinfo info = new Productssinfo();

                                info.Price = "" + data.GetInt32(2);
                                info.Name = data.GetString(1);
                                info.Category = data.GetString(3);
                                info.ImageData = data.GetSqlBinary(4).Value;
                                info.ProductID = data.GetInt32(0);
                                info.FreelancerID = data.GetInt32(5);

                                Products.Add(info);
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

        private void LoadServices()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT S.[Name], S.[Price], S.[Category], SI.[img], S.[ServiceID] FROM Service AS S\r\nJOIN ServiceIMG AS SI ON S.ServiceID = SI.ServiceID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                int serviceID = data.GetInt32(data.GetOrdinal("ServiceID"));
                                ServicesInfo info = new ServicesInfo();

                                info.Price = "" + data.GetInt32(1);
                                info.Name = data.GetString(0);
                                info.Category = data.GetString(2);
                                info.ImageData = data.GetSqlBinary(3).Value;
                                info.serviceID = data.GetInt32(4);

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

        public class Productssinfo
        {
            public string Name;
            public string Price;
            public int ProductID;
            public int FreelancerID;
            public string Category { get; set; }
            public byte[] ImageData { get; set; }
        }

        public class ServicesInfo
        {
            public string Name;
            public string Price;
            public int serviceID;
            public string Category { get; set; }
            public byte[] ImageData { get; set; }
        }
    }
}
