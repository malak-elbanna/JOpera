using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
namespace Project_test.Pages
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        [MinLength(3)]
        public string fname { get; set; }
        
        public string? lname { get; set; }

        public string? AmPm { get; set; }="no";
        public string? AmPm2 { get; set; } = "no";
        public string? WorkingHour { get; set; }
        public string? WorkingHour2 { get; set; }
        public string? freelancerProjectD { get; set; }
        public string? freelancerProjectN { get; set; }
        public string? FreelancerExperience { get; set; }
        public void OnGet()
        {

        }

        public IActionResult OnPostLogin(string email, string password)
        {
            string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

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
                            int Id = reader.GetInt32(reader.GetOrdinal("UserID"));
                            return RedirectToPage("/UserInfo", new { userId = Id });
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
        private bool IsEmailExists(string email, string connectionString)
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
        public IActionResult OnPostSignup(string fname, string lname, string role, string phone, DateTime birthdate, char gender, string email, string pass, string city, string street, string zipCode, string customerComm, string customerCategory, string customerPayment, string FreelancerExperience, string freelancerProjectN, string freelancerProjectD, string WorkingHour, string AmPm, string WorkingHour2, string AmPm2)
        {
            if (!ModelState.IsValid)
            {
                //TempData["ErrorMessage"] = "Invalid data entry, try again.";

                Console.WriteLine($"fname is: {fname}");

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        TempData["ErrorMessage"] = error.ErrorMessage;
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }

                return Page();
            }
            else
            {
                try
                {
                    string workingHours = $"From {WorkingHour} {AmPm} to {WorkingHour2} {AmPm2}";
                    string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
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
                                    freelancerCommand.Parameters.AddWithValue("@WorkExperience", FreelancerExperience);
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

                            return RedirectToPage("/Index");
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