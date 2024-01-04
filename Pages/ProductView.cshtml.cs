using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_test.Models;
using System.Data.SqlClient;

namespace Project_test.Pages
{
    public class ProductViewModel : PageModel
    {
        public SqlConnection? Con { get; set; }
        public string? Name { get; set; }
        public int? Price { get; set; }
        public int? FreelancerID { get; set; }
        public string? Description { get; set; }

        public string? Review { get; set; }
        public void OnGet()
        {
            GetProduct();
        }

        public void GetProduct()
        {
            //string conStr = "Data Source=DESKTOP-05RUH8H;Initial Catalog=JOperaF;Integrated Security=True";
            string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            //string conStr = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            //string conStr = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            
            Con = new SqlConnection(conStr);
            var productID = HttpContext.Session.GetInt32("ProductID");
            string ProductName = $"select Name from Product where ProductID = {productID} ";
            string ProductPrice = $"select Price from Product where ProductID = {productID}";
            string ProductFreelancerID = $"select FreelancerID from Product where ProductID = {productID}";
            string ProductDescription = $"select Description from Product where ProductID = {productID}";
            string Productrating = $"SELECT p.ProductID, r.Rating FROM Product p LEFT JOIN Reviews r ON p.ProductID = r.ProductID";
            string ProductReview = "$SELECT p.ProductID, p.Name, r.Comments FROM Product p LEFT JOIN Reviews r ON p.ProductID = r.ProductID;";

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

                using (SqlCommand cmd = new SqlCommand(ProductFreelancerID, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FreelancerID = Convert.ToInt32(reader["FreelancerID"]);

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

                using (SqlCommand cmd = new SqlCommand(ProductReview, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Review = reader["Review"].ToString();

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
        public void OnPost()
        {
            // Logic to save the review to the database
            if (!string.IsNullOrEmpty(Review))
            {
                //string conStr = "Data Source=DESKTOP-05RUH8H;Initial Catalog=JOperaF;Integrated Security=True";
                //string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                string conStr = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

                Con = new SqlConnection(conStr);
                using (SqlConnection Con = new SqlConnection(conStr))
                {
                    try
                    {
                        Con.Open();
                        string query = "INSERT INTO Reviews (Rating, Comments, ProductID) VALUES (@Rating, @Review, @ProductID)";
                        using (SqlCommand cmd = new SqlCommand(query, Con))
                        {
                            cmd.Parameters.AddWithValue("@Rating", Request.Form["rating"]); 
                            cmd.Parameters.AddWithValue("@Review", Review);
                           
                            cmd.Parameters.AddWithValue("@ProductID", HttpContext.Session.GetInt32("ProductID"));

                            cmd.ExecuteNonQuery();
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

            }
        }
    }
}
