using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Common;
using System.Data.SqlClient;


namespace Project_test.Pages
{
    public class FreelancerProfileModel : PageModel
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
        public List<string>? ServiceList { get; set; }
        public List<string>? ProjectNames { get; set; }
        public List<string>? ProjectDescriptions { get; set; }

        public void OnGet()
        {
            if (Request.Query.TryGetValue("FreelancerID", out var value))
            {
                string passedValue = value.ToString();
                GetFreelancer(passedValue);
            }
        }

        public void GetFreelancer(string ID)
        {
            //string conStr = "Data Source=DESKTOP-05RUH8H;Initial Catalog=JOperaF;Integrated Security=True";
            string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            //string conStr = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            //string conStr = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

            Con = new SqlConnection(conStr);

            string userQuery = $"select Fname, Lname, Email, Phone_number from Users where UserID = {ID}";
            string freelancerQuery = $"select Work_Experience, Working_Hours from Freelancers where FreelancerID = {ID}";
            string locationQuery = $"select City, Street_Num from Location where UserID = {ID}";
            string projectQuery = $"select Name, Description from Project where FreelancerID = {ID}";

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

                List<string>? projectNames = new List<string>();
                List<string>? projectDescriptions = new List<string>();

                using (SqlCommand cmd = new SqlCommand(projectQuery, Con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projectNames.Add(reader["Name"].ToString());
                            projectDescriptions.Add(reader["Description"].ToString());
                        }
                    }
                }

                ProjectNames = projectNames;
                ProjectDescriptions = projectDescriptions;

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

