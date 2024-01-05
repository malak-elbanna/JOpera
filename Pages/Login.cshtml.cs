using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_test.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
namespace Project_test.Pages
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        [Required]
        [MinLength(3,ErrorMessage ="First Name must be atleast 3 letters")]
        public string fname { get; set; }
        [Required]
        [MinLength(3,ErrorMessage = "Second Name must be atleast 3 letters")]
        public string? lname { get; set; }

        public string AmPm { get; set; }
        
        public string AmPm2 { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters and must contain atleast 1 uppercase, lowercase, number and a special character.")]
        public string pass { get; set; }
        
        [Required]
        [Compare(nameof(pass),ErrorMessage = "Passwords dont match")]
        public string pass2 { get; set; }
        [Required]
        public string zipCode { get; set; }
        [Required]
        [StringLength(11, ErrorMessage = "Invalid phone Number.")]
        [RegularExpression(@"^0[0-9]{10}$", ErrorMessage = "Phone number must start with '0'")]
        public string? phone { get; set; }
        [Required]
        public string? birthdate { get; set; }
        [Range(1, 12, ErrorMessage = "Working hour must be between 1 and 12.")]
        public string? WorkingHour { get; set; }
        [Range(1, 12, ErrorMessage = "Working hour must be between 1 and 12.")]
        public string? WorkingHours2 { get; set; }
        public string? freelancerProjectD { get; set; }
        public string? freelancerProjectN { get; set; }
        public string? Experience { get; set; }
        public string? customerComm { get; set; }
        public string? customerCategory { get; set; }
        public string? customerPayment { get; set; }
        public string? city { get; set; }
        public string? street { get; set; }
        public string? role { get; set; }


        public int? userId { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == 0|| userId==null)
            {

            }
            else
            {
                Console.WriteLine(userId);
                Response.Redirect("/Logout");
            }
        }

        public IActionResult OnPostLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Email and password are required.";
                return RedirectToPage("/Login");
            }
            //string connectionString = "Data Source=DESKTOP-05RUH8H;Initial Catalog=JOperaF;Integrated Security=True";
            //string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            //string connectionString = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            // string connectionString = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM Users WHERE Email = @Email AND Password = @Password";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            int userId = reader.GetInt32(reader.GetOrdinal("UserID"));
                            string userRole = reader.GetString(reader.GetOrdinal("Role"));
                            HttpContext.Session.SetInt32("UserId", userId);
                            HttpContext.Session.SetString("UserRole", userRole);
                            return RedirectToPage("/UserInfo");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Invalid email or password. Please try again.";
                            return RedirectToPage("/Login");
                        }
                    }
                }   
            }
        }
        public bool IsEmailExists(string email, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(*) FROM Users WHERE Email = @Email";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }
        public IActionResult OnPostSignup(string fname, string lname, string role, string phone, DateTime birthdate, char gender, string email, string pass, string city, string street, string zipCode, string customerComm, string customerCategory, string customerPayment, string Experience, string freelancerProjectN, string freelancerProjectD, string WorkingHour, string AmPm, string WorkingHours2, string AmPm2)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }

                return Page();
            }
            else
            {
                try
                {
                    string workingHours = $"From {WorkingHour} {AmPm} to {WorkingHours2} {AmPm2}";
                    //string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
                    string connectionString = "Data Source=Alasil;Initial Catalog=JOpera;Integrated Security=True";
                    //string connectionString = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
                    if (IsEmailExists(email, connectionString))
                    {
                        TempData["ErrorMessage"] = "Email already exists. Please use a different email address.";
                        return RedirectToPage("/Login");
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string UserQuery = "INSERT INTO Users (Fname, Lname, Role, Phone_number, Birth_date, Gender, Email, Password) VALUES (@Fname, @Lname, @Role, @Phone, @Birthdate, @Gender, @Email, @Password) SELECT SCOPE_IDENTITY();";

                        using (SqlCommand command = new SqlCommand(UserQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Fname", fname);
                            command.Parameters.AddWithValue("@Lname", lname);
                            command.Parameters.AddWithValue("@Role", role);
                            command.Parameters.AddWithValue("@Phone", phone);
                            command.Parameters.AddWithValue("@Birthdate", birthdate);
                            command.Parameters.AddWithValue("@Gender", gender);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Password", pass);

                            int userId = Convert.ToInt32(command.ExecuteScalar());

                            string insertLocationQuery = "INSERT INTO Location (City, Street_Num, Postal_Code, UserID) VALUES (@City, @Street, @PostalCode, @UserID)";

                            using (SqlCommand locationCommand = new SqlCommand(insertLocationQuery, connection))
                            {
                                locationCommand.Parameters.AddWithValue("@City", city);
                                locationCommand.Parameters.AddWithValue("@Street", street);
                                locationCommand.Parameters.AddWithValue("@PostalCode", zipCode);
                                locationCommand.Parameters.AddWithValue("@UserID", userId);
                                locationCommand.ExecuteNonQuery();
                            }

                            if (role == "freelancer")
                            {
                                string FreelancerQuery = "INSERT INTO Freelancers (Working_Hours, Work_Experience, FreelancerID) VALUES (@WorkingHours, @WorkExperience, @UserID);";

                                using (SqlCommand freelancerCommand = new SqlCommand(FreelancerQuery, connection))
                                {
                                    freelancerCommand.Parameters.AddWithValue("@WorkingHours", workingHours);
                                    freelancerCommand.Parameters.AddWithValue("@WorkExperience", Experience);
                                    freelancerCommand.Parameters.AddWithValue("@UserID", userId);
                                    freelancerCommand.ExecuteNonQuery();
                                }
                                string ProjectQuery = "INSERT INTO Project (FreelancerID, Name, Description) " +
                                "VALUES (@FreelancerID, @Name, @Description);";

                                using (SqlCommand projectCommand = new SqlCommand(ProjectQuery, connection))
                                {
                                    projectCommand.Parameters.AddWithValue("@FreelancerID", userId);
                                    projectCommand.Parameters.AddWithValue("@Name", freelancerProjectN);
                                    projectCommand.Parameters.AddWithValue("@Description", freelancerProjectD);
                                    projectCommand.ExecuteNonQuery();
                                }
                            }
                            else if (role == "customer")
                            {
                                string CustomerQuery = "INSERT INTO Customers (Preferred_Communication, Preferred_Category, Preferred_Payment_Method, CustomerID) " +
                                                             "VALUES (@Comm, @Category, @PaymentMethod, @UserID);";

                                using (SqlCommand customerCommand = new SqlCommand(CustomerQuery, connection))
                                {
                                    customerCommand.Parameters.AddWithValue("@Comm", customerComm);
                                    customerCommand.Parameters.AddWithValue("@Category", customerCategory);
                                    customerCommand.Parameters.AddWithValue("@PaymentMethod", customerPayment);
                                    customerCommand.Parameters.AddWithValue("@UserID", userId);
                                    customerCommand.ExecuteNonQuery();
                                }
                            }

                            return RedirectToPage("/Login");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["ErrorMessage"] = ex.Message;
                    return RedirectToPage("/Login");
                }
            }
        }
    }
}