using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Project_test.Models;
using Microsoft.Extensions.Primitives;

namespace Project_test.Pages
{
    public class ShopCartModel : PageModel
    {
        public ShopCartModel()
        {
            Products = new List<ProductModel>();
            Services = new List<ServiceModel>();
        }
        public List<ProductModel> Products { get; set; }
        public List<ServiceModel> Services { get; set; }
        public int Total { get; set; }
        public class ProductModel
        {
            public int ProductID { get; set; }
            public int Quantity { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public int Sum { get; set; }
        }
        public class ServiceModel
        {
            public int ServiceID { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public int Sum { get; set; }

        }
        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == 0 || userId == null)
            {
                Console.WriteLine("No user ID");
                Response.Redirect("/NotLogIn");
            }
            else
            {
                string connectionString = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                //string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                                 SELECT c.ProductID,c.Quantity, p.Name, p.price,sum(price) as sum
                                 FROM ProductCart c
                                 INNER JOIN Product p ON c.ProductID = p.ProductID
                                 WHERE c.CustomerID = @UserID
                                 Group by c.ProductID,c.Quantity, p.Name, p.price";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int productID = reader.GetInt32(reader.GetOrdinal("ProductID"));
                                    int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                                    string productName = reader.GetString(reader.GetOrdinal("Name"));
                                    int price = reader.GetInt32(reader.GetOrdinal("price"));
                                    int sum = reader.GetInt32(reader.GetOrdinal("sum"));


                                    Products.Add(new ProductModel
                                    {
                                        ProductID = productID,
                                        Quantity = quantity,
                                        Name = productName,
                                        Price = price,
                                        Sum = sum
                                    });
                                }
                            }
                        }
                    }
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                                 SELECT c.ServiceID,c.Name, c.price, sum(price) as sum
                                 FROM ServiceCart s
                                 INNER JOIN Service c ON s.ServiceID = c.ServiceID
                                 WHERE s.CustomerID = @UserID
                                 Group by c.ServiceID,c.Name, c.price";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int ServiceID = reader.GetInt32(reader.GetOrdinal("ServiceID"));
                                    string ServiceName = reader.GetString(reader.GetOrdinal("Name"));
                                    int price = reader.GetInt32(reader.GetOrdinal("price"));
                                    int sum = reader.GetInt32(reader.GetOrdinal("sum"));

                                    Services.Add(new ServiceModel
                                    {
                                        ServiceID = ServiceID,
                                        Name = ServiceName,
                                        Price = price,
                                        Sum = sum

                                    });
                                }
                            }
                        }
                    }
                }
                int Total = Products.Sum(p => p.Sum) + Services.Sum(s => s.Sum);
                this.Total = Total;
            }
        }
        /* public IActionResult OnPostUpdate()
         {
             //var userId = HttpContext.Session.GetInt32("UserId");

             //Console.WriteLine($"ID in UPDATE CART IS {userId}");

             //string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

             //using (SqlConnection connection = new SqlConnection(connectionString))
             //{
             //    connection.Open();

             //    string updateQuery = @"
             //    UPDATE ProductCart
             //    SET Quantity = @UpdatedQuantity
             //    WHERE ProductID = @UpdatedProductId AND CustomerID = @UserId";

             //    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
             //    {
             //        updateCommand.Parameters.AddWithValue("@UpdatedQuantity", UpdatedQuantity);
             //        updateCommand.Parameters.AddWithValue("@UpdatedProductId", UpdatedProductId);
             //        updateCommand.Parameters.AddWithValue("@UserId", userId);

             //        int rowsAffected = updateCommand.ExecuteNonQuery();

             //        if (rowsAffected > 0)
             //        {
             //            return RedirectToPage("/ShopCart");
             //        }
             //        else
             //        {
             //            return Page();
             //        }
             //    }
             //}
         }
     */
        public IActionResult OnPost()
        {
            var updatedProductId = int.Parse(Request.Form["updatedProductId"]);
            var updatedQuantity = Request.Form["updatedQuantity"];
            var userId = HttpContext.Session.GetInt32("UserId");

            string connectionString = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            int quantityToAdd = updatedQuantity == "increase" ? 1 : -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = @"
                        UPDATE ProductCart
                        SET Quantity = Quantity + @QuantityToAdd
                        WHERE ProductID = @UpdatedProductId AND CustomerID = @UserId";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@QuantityToAdd", quantityToAdd);
                    updateCommand.Parameters.AddWithValue("@UpdatedProductId", updatedProductId);
                    updateCommand.Parameters.AddWithValue("@UserId", userId);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return RedirectToPage("/ShopCart");
                    }
                    else
                    {
                        return Page();
                    }
                }
            }
        }
    }
}
