using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Project_test.Pages
{
    public class ServiceStatusModel : PageModel
    {
        public List<string>? Orders { get; set; }
        [BindProperty]
        public string? customerName { get; set; }
        [BindProperty]
        public string? Status { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            Orders = GetOrders();
        }

        public List<string> GetOrders()
        {
            List<string> customerNames = new List<string>();
            string conStr = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            //string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection Con = new SqlConnection(conStr))
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                string query = $@"SELECT DISTINCT Users.Fname, Users.Lname
                                  FROM Users
                                  JOIN Customers cu ON cu.CustomerID = Users.UserID
                                  JOIN Orders o ON o.CustomerID = cu.CustomerID
                                  JOIN contain c ON c.OrderID = o.OrderID
                                  JOIN Service s ON s.ServiceID = c.ServiceID
                                  JOIN Freelancers f ON f.FreelancerID = s.FreelancerID
                                  WHERE f.FreelancerID = {userId} AND o.Status != 'finished'";

                try
                {
                    Con.Open();

                    using (SqlCommand cmd = new SqlCommand(query, Con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string firstName = reader.GetString(0);
                            string lastName = reader.GetString(1);

                            string customerName = $"{firstName} {lastName}";
                            customerNames.Add(customerName);
                        }

                        reader.Close();
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return customerNames;
        }

        public IActionResult OnPostStatus()
        {
            string conStr = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            //string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFFFFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            using (SqlConnection Con = new SqlConnection(conStr))
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                string updateQuery = @"UPDATE Orders 
                        SET Status = @Status
                        WHERE OrderID IN (
                            SELECT o.OrderID 
                            FROM Users u 
                            JOIN Customers cu ON cu.CustomerID = u.UserID 
                            JOIN Orders o ON o.CustomerID = cu.CustomerID 
                            JOIN contain c ON c.OrderID = o.OrderID 
                            JOIN Service s ON s.ServiceID = c.ServiceID 
                            JOIN Freelancers f ON f.FreelancerID = s.FreelancerID 
                            WHERE f.FreelancerID = @UserId
                                AND o.Status != 'finished' 
                                AND CONCAT(u.Fname, ' ', u.Lname) = @CustomerName
                        )";

                Console.WriteLine("HI I AM NO WHERE");

                try
                {
                    Con.Open();
                    Console.WriteLine("HI I AM IN THE TRY");
                    Console.WriteLine(Status);
                    using (SqlCommand cmd = new SqlCommand(updateQuery, Con))
                    {
                        cmd.Parameters.AddWithValue("@Status", Status);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@CustomerName", customerName);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToPage("/ThankYou");
                        }
                        return RedirectToPage("/UserInfo");
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("HI I AM IN THE CATCH");
                    return RedirectToPage("/UserInfo");
                }
                finally
                {
                    Con.Close();
                }
            }
        }
    }
}