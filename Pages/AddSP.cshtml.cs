
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Project_test.Pages
{
    public class AddSPModel : PageModel
    {
        public void OnGet()
        {
        }

        public SqlConnection? Con { get; set; }
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

        public IActionResult OnPostAddSP()
        {
            string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            Con = new SqlConnection(conStr);

            string insertProductQuery = "INSERT INTO Product (Category, Name, Price, Description) VALUES (@Category, @Name, @Price, @Description); SELECT SCOPE_IDENTITY();";
            string insertProductImageQuery = "INSERT INTO ProductIMG (ProductID, img) VALUES (@ProductID, @ImageData);";

            try
            {
                Console.WriteLine("AddProduct method started...");
                Console.WriteLine($"Received data - Category: {category}, Name: {name}, Price: {price}, Description: {description}, Image count: {images.Count}");

                using (SqlCommand cmd = new SqlCommand(insertProductQuery, Con))
                {
                    Con.Open();
                    cmd.Parameters.Add("@Category", SqlDbType.VarChar).Value = category;
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@Price", SqlDbType.Int).Value = price;
                    cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;

                    int productId = Convert.ToInt32(cmd.ExecuteScalar());

                    if (productId > 0 && images != null && images.Any())
                    {
                        foreach (var image in images)
                        {
                            if (image.Length > 0)
                            {
                                byte[] imageData = null;
                                using (var stream = new MemoryStream())
                                {
                                    image.CopyTo(stream);
                                    imageData = stream.ToArray();
                                }

                                using (SqlCommand imageCmd = new SqlCommand(insertProductImageQuery, Con))
                                {
                                    imageCmd.Parameters.Add("@ProductID", SqlDbType.Int).Value = productId;
                                    imageCmd.Parameters.Add("@ImageData", SqlDbType.VarBinary).Value = imageData;
                                    imageCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("AddProduct method ended...");
                return RedirectToPage("/ThankYou");
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToPage("/UserInfo");
            }
            finally
            {
                Con.Close();
            }
        }
    }
}