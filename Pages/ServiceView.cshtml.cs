using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_test.Models;
using System.Data.SqlClient;

namespace Project_test.Pages
{
    public class ServiceViewModel : PageModel
    {
        public SqlConnection? Con { get; set; }
        public string? Name { get; set; }
        public int? Price { get; set; }
        public string? Description { get; set; }
        public string? FreelancerName { get; set; }
        public string? Review { get; set; }

        public float? Rating { get; set; }

        public List<byte[]>? Imagess { get; set; }
        public string? ServiceID { get; set; }


        public void OnGet()
        {

            if (Request.Query.TryGetValue("ServiceID", out var value))
            {
                // 'value' contains the value passed in the URL
                string passedValue = value.ToString();
                ServiceID = passedValue;
                Console.WriteLine(passedValue);
                GetProduct(passedValue);
                GetServiceImages(passedValue);
            }

        }

        


       
        public void GetServiceImages(string ServiceID)
        {
            //Console.WriteLine($"{ServiceID}");
            Imagess = new List<byte[]>();
            
            string conStr = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            string selectImagesQuery = $"SELECT img FROM ServiceIMG WHERE ServiceID = {ServiceID}";

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
                            Imagess.Add(imageBytes);
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

        public void GetProduct(string id)
        {
            //string conStr = "Data Source=DESKTOP-05RUH8H;Initial Catalog=joperaffff;Integrated Security=True";
            // string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            //string conStr = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            string conStr = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";

            Con = new SqlConnection(conStr);
            var serviceID = id;
            string serviceName = $"select Name from Product where ProductID = {serviceID} ";  
            string servicePrice = $"select Price from Product where ProductID = {serviceID}";  
            string serviceDescription = $"select Description from Product where ProductID = {serviceID}";
            string servicerating = $"SELECT Rating FROM Reviews WHERE OrderID = (  SELECT OrderID   FROM contain  WHERE ProductID ={serviceID});"; 
            string serviceReview = $"SELECT Comments FROM Reviews WHERE OrderID = (    SELECT OrderID   FROM contain    WHERE ProductID = {serviceID});"; 
            string serviceFreeLancerName = $"SELECT    u.Fname  AS FreelancerName FROM    Product p JOIN   Freelancers f ON p.FreelancerID = f.FreelancerID JOIN    Users u ON f.FreelancerID = u.UserID WHERE    p.ProductID =  {serviceID};"; 
            try
            {
                Con.Open();

                using (SqlCommand cmd = new SqlCommand(serviceName, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Name = $"{reader["Name"]}";

                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(servicePrice, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Price = Convert.ToInt32(reader["Price"]);

                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(servicerating, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Rating = Convert.ToInt32(reader["Rating"]);

                        }
                    }
                }


                using (SqlCommand cmd = new SqlCommand(serviceDescription, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Description = reader["Description"].ToString();

                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand( serviceFreeLancerName, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FreelancerName = reader["FreelancerName"].ToString();

                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(serviceReview, Con))
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





    }
}