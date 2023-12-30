using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;
namespace Project_test.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
            string connectionString = "Data Source=bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            string query1 = "";
            SqlCommand countCommand = new SqlCommand(query1, connection);
            try
            {
                using (SqlDataReader reader = countCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally { connection.Close(); }

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
                            return RedirectToPage("/Login");
                        }
                    }
                }   
            }
        }

        public IActionResult OnPostSignup(string fname, string lname, string role, string phone, DateTime birthdate, char gender, string email,string pass, string city, string street, string zipCode, string customerComm, string customerCategory, string customerPayment, string FreelancerExperience, string freelancerProjectN, string freelancerProjectD,int WorkingHours)
        {
            try
            {
                string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";

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
                            string FreelancerQuery = "INSERT INTO Freelancers (Category, Working_Hours, Work_Experience, FreelancerID) VALUES (@Category, @WorkingHours, @WorkExperience, @UserID);";

                            using (SqlCommand freelancerCommand = new SqlCommand(FreelancerQuery, connection))
                            {
                                freelancerCommand.Parameters.AddWithValue("@Category", "null");
                                freelancerCommand.Parameters.AddWithValue("@WorkingHours", WorkingHours);
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
                return RedirectToPage("/Login");
            }
        }
    }
}