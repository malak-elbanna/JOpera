using iText.Layout.Borders;
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
        public class ReviewModel
        {
            public int Rating { get; set; }
            public string Comment { get; set; }
        }
        [BindProperty]
        public int ReviewRating { get; set; }

        [BindProperty]
        public string ReviewComment { get; set; }
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
        [BindProperty]
        public bool ProductInUserOrders { get; set; }
        public List<ReviewModel> Reviews { get; set; } = new List<ReviewModel>();

        string conStr = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
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
                ProductInUserOrders = IsProductInUserOrders(userId ?? 1, int.Parse(ProductID));
                Reviews = GetProductReviews(ProductID);
            }

        }
        public List<ReviewModel> GetProductReviews(string productId)
        {
            List<ReviewModel> reviews = new List<ReviewModel>();

            string selectReviewsQuery = $@"
SELECT Rating, Comments
FROM Reviews
WHERE ProductID = {productId}";

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(selectReviewsQuery, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int rating = Convert.ToInt32(reader["Rating"]);
                        string comment = reader["Comments"].ToString();
                        reviews.Add(new ReviewModel { Rating = rating, Comment = comment });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return reviews;
        }
        public void GetProductImages(string productId)
        {
            ProductID = productId;
            Images = new List<byte[]>();


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
        public bool IsProductInUserOrders(int userId, int productId)
        {
            List<int> orderIds = GetOrderIdsByUserId(userId);

                foreach (int orderId in orderIds)
            {
                if (IsProductInOrder(userId, orderId, productId))
                {
                    return true;
                }
            }
            return false;
        }
        private List<int> GetOrderIdsByUserId(int userId)
        {
            string selectOrdersQuery = @"
SELECT OrderID
FROM Orders
WHERE CustomerID = @CustomerId";

            List<int> orderIds = new List<int>();


            using (SqlConnection connection = new SqlConnection(conStr))
            {
                using (SqlCommand command = new SqlCommand(selectOrdersQuery, connection))
                {
                    command.Parameters.AddWithValue("@CustomerId", userId);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("OrderID")))
                                {
                                    int orderId = reader.GetInt32(reader.GetOrdinal("OrderID"));
                                    orderIds.Add(orderId);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return orderIds;
        }

        private bool IsProductInOrder(int userId, int orderId, int productId)
        {
            string selectContainQuery = $"SELECT ProductID FROM Contain WHERE OrderID = {orderId}";

            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(selectContainQuery, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            int productInOrder = Convert.ToInt32(reader["ProductID"]);
                            if (productInOrder == productId)
                            {
                                Console.WriteLine("FOUND ORDEEERRRRR");
                                return true;
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine("didnt find any....");
            return false;
        }

        public IActionResult OnPostReview(string ProductID)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var orderIds = GetOrderIdsByUserId(userId ?? 1);

                int orderId = orderIds[0];

                string insertReviewQuery = @"
INSERT INTO Reviews (Rating, Comments,CustomerID, OrderID,ProductID)
VALUES (@Rating, @Comments,@CustomerID, @OrderID,@ProductID);";

                using (SqlConnection connection = new SqlConnection(conStr))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(insertReviewQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Rating", ReviewRating);
                        command.Parameters.AddWithValue("@Comments", ReviewComment);
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        command.Parameters.AddWithValue("@CustomerID", userId);
                        command.Parameters.AddWithValue("@ProductID", ProductID);

                        command.ExecuteNonQuery();
                    }
                }

                TempData["ReviewMessage"] = "Review submitted successfully!";
                return RedirectToPage("/Products");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ReviewMessage"] = "Error submitting the review. Please try again.";
                return RedirectToPage("/ProductView");
            }
        }
    }
}