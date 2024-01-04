using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Project_test.Pages
{
    public class AddSP2Model : PageModel
    {
        public void OnGet()
        {
        }

        public SqlConnection? Con2 { get; set; }
        public string? type { get; set; } 
        [BindProperty]
        public string? name { get; set; } 
        [BindProperty]
        public string? category { get; set; } 
        [BindProperty]
        public int price { get; set; } 
        [BindProperty]
        public List<IFormFile>? images { get; set; } 
        [BindProperty]
        public string? description { get; set; } 

        public IActionResult OnPostAddSP2()
        {
            //string conStr2 = "Data Source=DESKTOP-05RUH8H;Initial Catalog=JOperaF;Integrated Security=True"
            //string conStr2 = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            //string conStr2 = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            string conStr2 = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            Con2 = new SqlConnection(conStr2);

            var userId = HttpContext.Session.GetInt32("UserId");
            string insertServiceQuery = $"INSERT INTO Service (Category, Name, Price, Description, FreelancerID) VALUES (@Category, @Name, @Price, @Description, {userId}); SELECT SCOPE_IDENTITY();";
            string insertServiceImageQuery = "INSERT INTO ServiceIMG (serviceID, img) VALUES (@ServiceID, @ImageData);";

            try
            {
                Console.WriteLine("AddProduct method started...");
                Console.WriteLine($"Received data - Category: {category}, Name: {name}, Price: {price}, Description: {description}, Image count: {images.Count}");

                using (SqlCommand cmd = new SqlCommand(insertServiceQuery, Con2))
                {
                    Con2.Open();
                    cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = category;
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@Price", SqlDbType.Int).Value = price;
                    cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;

                    int serviceId = Convert.ToInt32(cmd.ExecuteScalar());

                    if (serviceId > 0 && images != null && images.Any())
                    {
                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                byte[]? imageData = null;
                                using (var stream = new MemoryStream())
                                {
                                    image.CopyTo(stream);
                                    imageData = stream.ToArray();
                                }

                                using (SqlCommand imageCmd = new SqlCommand(insertServiceImageQuery, Con2))
                                {
                                    imageCmd.Parameters.Add("@ServiceID", SqlDbType.Int).Value = serviceId;
                                    imageCmd.Parameters.Add("@ImageData", SqlDbType.VarBinary).Value = imageData;
                                    imageCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("AddService method ended...");
                return RedirectToPage("/ThankYou");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToPage("/UserInfo");
            }
            finally
            {
                Con2.Close();
            }
        }
    }
}
