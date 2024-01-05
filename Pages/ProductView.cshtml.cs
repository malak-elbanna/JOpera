using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_test.Models;
using System.Data;
using System.Data.SqlClient;

namespace Project_test.Pages
{
    public class ProductViewModel : PageModel
    {
        public int? userId { get; set; }

        public SqlConnection? Con { get; set; }
        public string? Name { get; set; }
        public int? Price { get; set; }
        public string? Description { get; set; }
        public string? FreelancerName { get; set; }
        public string? Review { get; set; }
        public List<byte[]>? Images { get; set; }

        public float? Rating { get; set; }
        public string? ProductID { get; set; }
        public string? FreelancerID { get; set; }



        public void OnGet()
        {
            if (Request.Query.TryGetValue("FreelancerID", out var value2))
            {
                FreelancerID=value2.ToString();
            }

            if (Request.Query.TryGetValue("ProductID", out var value))
            {

                string passedValue = value.ToString();
                ProductID = passedValue;
                Console.WriteLine(passedValue);
                GetProduct(passedValue);
                GetProductImages(passedValue);
            }

        }
        public void GetProductImages(string productId)
        {
            ProductID = productId;
            Images = new List<byte[]>();

            string conStr = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";
            //string conStr = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            //string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            string selectImagesQuery = $"SELECT img FROM ProductIMG WHERE ProductID = {productId}";

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(selectImagesQuery, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            byte[] imageBytes = (byte[])reader["img"];
                            Images.Add(imageBytes);
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public IActionResult OnPostAddToCart(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == 0 || userId == null)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                try
                {

                    string conStr = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";
                    using (var connection = new SqlConnection(conStr))
                    {
                        connection.Open();

                        string insertQuery = "INSERT INTO ProductCart (CustomerID, ProductID, Quantity) VALUES (@CustomerID, @ProductID, 1)";
                        using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                        {
                            

                            cmd.Parameters.AddWithValue("@CustomerID", userId);
                            cmd.Parameters.AddWithValue("@ProductID", productId);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                return RedirectToPage("/ShopCart");
                            }
                            else
                            {
                                return RedirectToPage("/Error");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return RedirectToPage("/Error");
                }
            }
        }

        


        public void GetProduct(string id)
        {
            //string conStr = "Data Source=DESKTOP-05RUH8H;Initial Catalog=joperaffff;Integrated Security=True";
            //string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            //string conStr = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            string conStr = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";

            Con = new SqlConnection(conStr);
            var productID = id;
            string ProductName = $"select Name from Product where ProductID = {productID} ";  //done
            string ProductPrice = $"select Price from Product where ProductID = {productID}";  //done
            string ProductDescription = $"select Description from Product where ProductID = {productID}";//done
            string Productrating = $"SELECT Rating FROM Reviews WHERE OrderID = (  SELECT OrderID   FROM contain  WHERE ProductID ={productID});"; //done
            string ProductReview = $"SELECT Comments FROM Reviews WHERE OrderID = (    SELECT OrderID   FROM contain    WHERE ProductID = {productID});"; //done
            string ProductFreeLancerName = $"SELECT    u.Fname  AS FreelancerName FROM    Product p JOIN   Freelancers f ON p.FreelancerID = f.FreelancerID JOIN    Users u ON f.FreelancerID = u.UserID WHERE    p.ProductID =  {productID};"; //done

            try
            {
                Con.Open();

                using (SqlCommand cmd = new SqlCommand(ProductName, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Name = $"{reader["Name"]}";

                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(ProductPrice, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Price = Convert.ToInt32(reader["Price"]);

                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(Productrating, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Rating = Convert.ToInt32(reader["Rating"]);

                        }
                    }
                }


                using (SqlCommand cmd = new SqlCommand(ProductDescription, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Description = reader["Description"].ToString();

                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand(ProductFreeLancerName, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FreelancerName = reader["FreelancerName"].ToString();

                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(ProductReview, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Review = reader["Comments"].ToString();

                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Con.Close();
            }
        }

        /*public IActionResult OnPostUpdateQuantity(string updatedProductId, string action)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            //string connectionString = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Con.Close();
            }

        }*/





    }
}