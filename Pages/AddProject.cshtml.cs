using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace Project_test.Pages
{
    public class AddProjectModel : PageModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? userId { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
        }
        public void OnPost()
        {
            try
            {
                //string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
                string connectionString = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Project" + "(FreeLancerID, Name, Description) VALUES" + "" +
                        "(@FreeLancerID, @Name, @Description)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        //command.Parameters.AddWithValue("@FreeLancerID", 100);
                        command.Parameters.AddWithValue("@Name", Request.Form["name"].ToString());
                        var userId = HttpContext.Session.GetInt32("UserId");
                        command.Parameters.AddWithValue("@FreeLancerID", userId);

                        command.Parameters.AddWithValue("@Description", Request.Form["Description"].ToString());

                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                
            }

            Response.Redirect("/ThankYou");
        }
    }
}
