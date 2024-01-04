using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;


namespace Project_test.Pages
{
    public class FreeLancer_infoModel : PageModel
    {
        public SqlConnection? Con { get; set; }
        public string? FreelancerName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WorkExperience { get; set; }
        public string? WorkingHours { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Location { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? Role { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userId == 0)
            {
                Console.WriteLine("LOGGED OUT");
            }
            else
            {
                GetFreelancer();
                Console.WriteLine("LOGGED IN WITH ID:");
                Console.WriteLine(userId);
                Console.WriteLine("Role :");
                Console.WriteLine(userRole);
                Role = userRole;
            }
        }
        public void GetFreelancer()
        {
            
            string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            //string conStr = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            Con = new SqlConnection(conStr);

            var userId = HttpContext.Session.GetInt32("UserId");
            string userQuery = $"select Fname, Lname, Email, Phone_number from Users where UserID = {userId}";
            string freelancerQuery = $"select Work_Experience, Working_Hours from Freelancers where FreelancerID = {userId}";
            string locationQuery = $"select City, Street_Num from Location where UserID = {userId}";
            string projectQuery = $"select Name, Description from Project where FreelancerID = {userId}";
            string serviceQuery = $"select ServiceID ";

            try
            {
                Con.Open();

                using (SqlCommand cmd = new SqlCommand(userQuery, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FreelancerName = $"{reader["Fname"]} {reader["Lname"]}";
                            Email = reader["Email"].ToString();
                            PhoneNumber = reader["Phone_number"].ToString();
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(freelancerQuery, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            WorkExperience = reader["Work_Experience"].ToString();
                            WorkingHours = reader["Working_Hours"].ToString();
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(locationQuery, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            City = reader["City"].ToString();
                            Street = reader["Street_Num"].ToString();
                            Location = Street + ", " + City;
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(projectQuery, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ProjectName = reader["Name"].ToString();
                            ProjectDescription = reader["Description"].ToString();
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
