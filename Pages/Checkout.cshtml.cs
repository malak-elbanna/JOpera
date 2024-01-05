using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_test.Models;
using System.Data.SqlClient;
using System;
using static Project_test.Pages.ShopCartModel;
using System.IO;
using Org.BouncyCastle.Security;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace Project_test.Pages
{
    [BindProperties]
    public class CheckoutModel : PageModel
    {
        public int? userId { get; set; }

        public bool GenerateReceipt { get; set; }
        public string? city { get; set; }
        public string? street { get; set; }
        public string zipCode { get; set; }
        public List<ProductModel> Products { get; set; }
        public List<ServiceModel> Services { get; set; }
        public UserModel User { get; set; }
        public List<LocationModel> UserLocations { get; set; }
        public CheckoutModel()
        {
            Products = new List<ProductModel>();
            Services = new List<ServiceModel>();
            UserLocations = new List<LocationModel>();
            User = new UserModel();
        }
        public int Total { get; set; }
        public class ProductModel
        {
            public int ProductID { get; set; }
            public int Quantity { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public int Sum { get; set; }
        }
        public class ServiceModel
        {
            public int ServiceID { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public int Sum { get; set; }

        }
        public class UserModel
        {
            public string Fname { get; set; }
            public string Lname { get; set; }
            public string Phone_Number { get; set; }
            public string Email { get; set; }
            public string Birth_Date { get; set; }
            public string Gender { get; set; }
        }

        public class LocationModel
        {
            public string City { get; set; }
            public string Street_Num { get; set; }
            public string Postal_Code { get; set; }
        }
        public class OrderModel
        {
            public int OrderID { get; set; }
            public DateTime Order_Date { get; set; }
            public string SpecificTime { get; set; }
            public int PaymentID { get; set; }

            // Add other properties as needed
        }
        public class PaymentModel
        {
            public string Method { get; set; }
        }
        //string connectionString = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
        string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == 0 || userId == null)
            {
                Response.Redirect("/NotLogIn");
            }
            UserModel user;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string userQuery = @"
            SELECT Fname, Lname, Phone_Number, Email
            FROM Users
            WHERE UserID = @UserID";

                using (SqlCommand userCommand = new SqlCommand(userQuery, connection))
                {
                    userCommand.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader userReader = userCommand.ExecuteReader())
                    {
                        userReader.Read();

                        user = new UserModel
                        {
                            Fname = userReader.GetString(userReader.GetOrdinal("Fname")),
                            Lname = userReader.GetString(userReader.GetOrdinal("Lname")),
                            Phone_Number = userReader.GetString(userReader.GetOrdinal("Phone_Number")),
                            Email = userReader.GetString(userReader.GetOrdinal("Email")),
                        };
                    }
                }
            }
            List<LocationModel> userLocations = new List<LocationModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string locationQuery = @"
            SELECT City, Street_Num, Postal_Code
            FROM Location
            WHERE UserID = @UserID";

                using (SqlCommand locationCommand = new SqlCommand(locationQuery, connection))
                {
                    locationCommand.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader locationReader = locationCommand.ExecuteReader())
                    {
                        while (locationReader.Read())
                        {
                            LocationModel location = new LocationModel
                            {
                                City = locationReader.GetString(locationReader.GetOrdinal("City")),
                                Street_Num = locationReader.GetString(locationReader.GetOrdinal("Street_Num")),
                                Postal_Code = locationReader.GetString(locationReader.GetOrdinal("Postal_Code")),
                            };

                            userLocations.Add(location);
                        }
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                                 SELECT c.ProductID,c.Quantity, p.Name, p.price,sum(price) as sum
                                 FROM ProductCart c
                                 INNER JOIN Product p ON c.ProductID = p.ProductID
                                 WHERE c.CustomerID = @UserID
                                 Group by c.ProductID,c.Quantity, p.Name, p.price ORDER BY c.ProductID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int productID = reader.GetInt32(reader.GetOrdinal("ProductID"));
                                int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                                string productName = reader.GetString(reader.GetOrdinal("Name"));
                                int price = reader.GetInt32(reader.GetOrdinal("price"));
                                int sum = reader.GetInt32(reader.GetOrdinal("sum"));


                                Products.Add(new ProductModel
                                {
                                    ProductID = productID,
                                    Quantity = quantity,
                                    Name = productName,
                                    Price = price,
                                    Sum = sum * quantity
                                });
                            }
                        }
                    }
                }
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                                 SELECT c.ServiceID,c.Name, c.price, sum(price) as sum
                                 FROM ServiceCart s
                                 INNER JOIN Service c ON s.ServiceID = c.ServiceID
                                 WHERE s.CustomerID = @UserID
                                 Group by c.ServiceID,c.Name, c.price";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ServiceID = reader.GetInt32(reader.GetOrdinal("ServiceID"));
                                string ServiceName = reader.GetString(reader.GetOrdinal("Name"));
                                int price = reader.GetInt32(reader.GetOrdinal("price"));
                                int sum = reader.GetInt32(reader.GetOrdinal("sum"));

                                Services.Add(new ServiceModel
                                {
                                    ServiceID = ServiceID,
                                    Name = ServiceName,
                                    Price = price,
                                    Sum = sum

                                });
                            }
                        }
                    }
                }
            }
            int total = Products.Sum(p => p.Sum) + Services.Sum(s => s.Sum);
            Total = total;
            User = user;
            UserLocations = userLocations;
        }

        public IActionResult OnPostAddLocation()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            //string connectionString = "Data Source=Alasil;Initial Catalog=JOperaFFFFF;Integrated Security=True";
            string connectionString = "Data Source=Bayoumi;Initial Catalog=JOpera;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = @"
            INSERT INTO Location (UserID, City, Street_Num, Postal_Code)
            VALUES (@UserID, @City, @Street, @PostalCode)";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@UserID", userId);
                    insertCommand.Parameters.AddWithValue("@City", city);
                    insertCommand.Parameters.AddWithValue("@Street", street);
                    insertCommand.Parameters.AddWithValue("@PostalCode", zipCode);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return RedirectToPage("/Checkout");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Could not add Location.";
                        return RedirectToPage("/Checkout");
                    }
                }
            }
        }

        public IActionResult OnPostDelete()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (GenerateReceipt)
            {
                Console.WriteLine("USER CHECKED IIITT");
                GenerateOrderPDF(userId ?? 1);
            }
            else
            {
                Console.WriteLine("el user tl3 5wl....");
            }
            DateTime now = DateTime.Now;
            string date = $"{now.Year}-{now.Month:D2}-{now.Day:D2}";
            string time = $"{now.Hour:D2}:{now.Minute:D2}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertPaymentQuery = @"
            INSERT INTO Payment (Method, Paid)
            VALUES ('Cash', 'f');
            SELECT SCOPE_IDENTITY()";

                int paymentId;
                using (SqlCommand insertPaymentCommand = new SqlCommand(insertPaymentQuery, connection))
                {
                    paymentId = Convert.ToInt32(insertPaymentCommand.ExecuteScalar());
                }
                string insertOrderQuery = @"
            INSERT INTO Orders (Status, Order_date, CustomerID, PaymentID, SpecificTime)
            VALUES ('In Progress', @OrderDate, @CustomerID, @PaymentID, @SpecificTime);
            SELECT SCOPE_IDENTITY()";

                int orderId;
                using (SqlCommand insertOrderCommand = new SqlCommand(insertOrderQuery, connection))
                {
                    insertOrderCommand.Parameters.AddWithValue("@OrderDate", date);
                    insertOrderCommand.Parameters.AddWithValue("@CustomerID", userId);
                    insertOrderCommand.Parameters.AddWithValue("@PaymentID", paymentId);
                    insertOrderCommand.Parameters.AddWithValue("@SpecificTime", time);

                    orderId = Convert.ToInt32(insertOrderCommand.ExecuteScalar());
                }
                using (SqlConnection connection1 = new SqlConnection(connectionString))
                {
                    connection1.Open();
                    string query = @"
                                 SELECT c.ProductID,c.Quantity, p.Name, p.price,sum(price) as sum
                                 FROM ProductCart c
                                 INNER JOIN Product p ON c.ProductID = p.ProductID
                                 WHERE c.CustomerID = @UserID
                                 Group by c.ProductID,c.Quantity, p.Name, p.price ORDER BY c.ProductID";

                    using (SqlCommand command = new SqlCommand(query, connection1))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int productID = reader.GetInt32(reader.GetOrdinal("ProductID"));

                                    Products.Add(new ProductModel
                                    {
                                        ProductID = productID,
                                    });
                                }
                            }
                        }
                    }
                }
                using (SqlConnection connection1 = new SqlConnection(connectionString))
                {
                    connection1.Open();
                    string query = @"
                                 SELECT c.ServiceID,c.Name, c.price, sum(price) as sum
                                 FROM ServiceCart s
                                 INNER JOIN Service c ON s.ServiceID = c.ServiceID
                                 WHERE s.CustomerID = @UserID
                                 Group by c.ServiceID,c.Name, c.price";

                    using (SqlCommand command = new SqlCommand(query, connection1))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int ServiceID = reader.GetInt32(reader.GetOrdinal("ServiceID"));

                                    Services.Add(new ServiceModel
                                    {
                                        ServiceID = ServiceID,
                                    });
                                }
                            }
                        }
                    }
                }

                string insertContainQuery = @"
            INSERT INTO Contain (OrderID, ProductID, ServiceID)
            VALUES (@OrderID, @ProductID, @ServiceID)";

                using (SqlCommand insertContainCommand = new SqlCommand(insertContainQuery, connection))
                {
                    insertContainCommand.Parameters.AddWithValue("@OrderID", orderId);

                    foreach (var product in Products)
                    {
                        insertContainCommand.Parameters.Clear();

                        insertContainCommand.Parameters.AddWithValue("@OrderID", orderId);

                        insertContainCommand.Parameters.AddWithValue("@ProductID", product.ProductID);

                        insertContainCommand.Parameters.AddWithValue("@ServiceID", DBNull.Value);

                        insertContainCommand.ExecuteNonQuery();
                    }

                    foreach (var service in Services)
                    {
                        insertContainCommand.Parameters.Clear();

                        insertContainCommand.Parameters.AddWithValue("@OrderID", orderId);

                        insertContainCommand.Parameters.AddWithValue("@ProductID", DBNull.Value);

                        insertContainCommand.Parameters.AddWithValue("@ServiceID", service.ServiceID);

                        insertContainCommand.ExecuteNonQuery();
                    }
                }
                string deleteProductCartQuery = @"
            DELETE FROM ProductCart WHERE CustomerID = @CustomerID";

                string deleteServiceCartQuery = @"
            DELETE FROM ServiceCart WHERE CustomerID = @CustomerID";
                using (SqlCommand deleteProductCartCommand = new SqlCommand(deleteProductCartQuery, connection))
                using (SqlCommand deleteServiceCartCommand = new SqlCommand(deleteServiceCartQuery, connection))
                {
                    deleteProductCartCommand.Parameters.AddWithValue("@CustomerID", userId);
                    deleteServiceCartCommand.Parameters.AddWithValue("@CustomerID", userId);

                    deleteProductCartCommand.ExecuteNonQuery();
                    deleteServiceCartCommand.ExecuteNonQuery();
                }
            }
            return RedirectToPage("/OrderDone");
        }

        public IActionResult GenerateOrderPDF(int userId)
        {
            UserModel user;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string userQuery = @"
                SELECT Fname, Lname, Phone_Number, Birth_Date, Gender, Email
                FROM Users
                WHERE UserID = @UserID";

                using (SqlCommand userCommand = new SqlCommand(userQuery, connection))
                {
                    userCommand.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader userReader = userCommand.ExecuteReader())
                    {
                        if (userReader.Read())
                        {
                            user = new UserModel
                            {
                                Fname = userReader.GetString(userReader.GetOrdinal("Fname")),
                                Lname = userReader.GetString(userReader.GetOrdinal("Lname")),
                                Phone_Number = userReader.GetString(userReader.GetOrdinal("Phone_Number")),
                                Email = userReader.GetString(userReader.GetOrdinal("Email")),
                                Birth_Date = userReader.GetDateTime(userReader.GetOrdinal("Birth_Date")).ToString("yyyy-MM-dd"),
                                Gender = userReader.GetString(userReader.GetOrdinal("Gender"))
                            };
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            // Retrieve user locations from the Location table
            List<LocationModel> userLocations = new List<LocationModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string locationQuery = @"
                SELECT City, Street_Num, Postal_Code, LocationID
                FROM Location
                WHERE UserID = @UserID";

                using (SqlCommand locationCommand = new SqlCommand(locationQuery, connection))
                {
                    locationCommand.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader locationReader = locationCommand.ExecuteReader())
                    {
                        while (locationReader.Read())
                        {
                            LocationModel location = new LocationModel
                            {
                                City = locationReader.GetString(locationReader.GetOrdinal("City")),
                                Street_Num = locationReader.GetString(locationReader.GetOrdinal("Street_Num")),
                                Postal_Code = locationReader.GetString(locationReader.GetOrdinal("Postal_Code")),
                                // Add other location properties
                            };

                            userLocations.Add(location);
                        }
                    }
                }
            }

            // Retrieve order details from the Orders table
            OrderModel order;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string orderQuery = @"
                SELECT TOP 1 OrderID, Order_Date, SpecificTime, PaymentID
                FROM Orders
                WHERE CustomerID = @UserID
                ORDER BY OrderID DESC";

                using (SqlCommand orderCommand = new SqlCommand(orderQuery, connection))
                {
                    orderCommand.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader orderReader = orderCommand.ExecuteReader())
                    {
                        if (orderReader.Read())
                        {
                            order = new OrderModel
                            {
                                OrderID = orderReader.GetInt32(orderReader.GetOrdinal("OrderID")),
                                Order_Date = orderReader.GetDateTime(orderReader.GetOrdinal("Order_Date")),
                                SpecificTime = orderReader.GetTimeSpan(orderReader.GetOrdinal("SpecificTime")).ToString(),
                                PaymentID = orderReader.GetInt32(orderReader.GetOrdinal("PaymentID")),
                            };
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }

            // Retrieve payment details from the Payment table
            PaymentModel payment;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string paymentQuery = @"
                SELECT Method
                FROM Payment
                WHERE PaymentID = @PaymentID";

                using (SqlCommand paymentCommand = new SqlCommand(paymentQuery, connection))
                {
                    paymentCommand.Parameters.AddWithValue("@PaymentID", order.PaymentID);

                    using (SqlDataReader paymentReader = paymentCommand.ExecuteReader())
                    {
                        if (paymentReader.Read())
                        {
                            payment = new PaymentModel
                            {
                                Method = paymentReader.GetString(paymentReader.GetOrdinal("Method")),
                                // Add other payment properties
                            };
                        }
                        else
                        {
                            // Handle the case when no payment details are found
                            return NotFound();
                        }
                    }
                }
            }
            List<ProductModel> products = new List<ProductModel>();
            List<ServiceModel> services = new List<ServiceModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string combinedQuery = @"
        SELECT C.ProductID, C.ServiceID, P.Name AS ProductName, P.Price AS ProductPrice, S.Name AS ServiceName, S.Price AS ServicePrice
        FROM Contain C
        LEFT JOIN Product P ON C.ProductID = P.ProductID
        LEFT JOIN Service S ON C.ServiceID = S.ServiceID
        WHERE C.OrderID = @OrderID";

                using (SqlCommand combinedCommand = new SqlCommand(combinedQuery, connection))
                {
                    combinedCommand.Parameters.AddWithValue("@OrderID", order.OrderID);

                    using (SqlDataReader combinedReader = combinedCommand.ExecuteReader())
                    {
                        while (combinedReader.Read())
                        {
                            int? productID = combinedReader.IsDBNull(combinedReader.GetOrdinal("ProductID"))
                                ? (int?)null
                                : combinedReader.GetInt32(combinedReader.GetOrdinal("ProductID"));

                            int? serviceID = combinedReader.IsDBNull(combinedReader.GetOrdinal("ServiceID"))
                                ? (int?)null
                                : combinedReader.GetInt32(combinedReader.GetOrdinal("ServiceID"));

                            if (productID.HasValue)
                            {
                                ProductModel product = new ProductModel
                                {
                                    ProductID = productID.Value,
                                    Name = combinedReader.IsDBNull(combinedReader.GetOrdinal("ProductName")) ? null : combinedReader.GetString(combinedReader.GetOrdinal("ProductName")),
                                    Price = combinedReader.IsDBNull(combinedReader.GetOrdinal("ProductPrice")) ? 0 : combinedReader.GetInt32(combinedReader.GetOrdinal("ProductPrice")),
                                    Quantity = combinedReader.IsDBNull(combinedReader.GetOrdinal("Quantity")) ? 0 : combinedReader.GetInt32(combinedReader.GetOrdinal("Quantity"))
                                };

                                products.Add(product);
                            }
                            else if (serviceID.HasValue)
                            {
                                ServiceModel service = new ServiceModel
                                {
                                    ServiceID = serviceID.Value,
                                    Name = combinedReader.IsDBNull(combinedReader.GetOrdinal("ServiceName")) ? null : combinedReader.GetString(combinedReader.GetOrdinal("ServiceName")),
                                    Price = combinedReader.IsDBNull(combinedReader.GetOrdinal("ServicePrice")) ? 0 : combinedReader.GetInt32(combinedReader.GetOrdinal("ServicePrice"))
                                };

                                services.Add(service);
                            }
                        }
                    }
                }
            }


            // Generate PDF
            string pdfFileName = $"Order_{order.OrderID}.pdf";
            string pdfFilePath = Path.Combine("D:\\Receipts", pdfFileName);

            using (PdfWriter writer = new PdfWriter(pdfFilePath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    // Add user information
                    document.Add(new Paragraph($"User Information: {user.Fname} {user.Lname}"));
                    document.Add(new Paragraph($"Phone Number: {user.Phone_Number}"));
                    document.Add(new Paragraph($"Email: {user.Email}"));
                    document.Add(new Paragraph($"Birth Date: {user.Birth_Date}"));
                    document.Add(new Paragraph($"Gender: {user.Gender}"));

                    // Add user locations
                    document.Add(new Paragraph("User Locations:"));
                    foreach (var location in userLocations)
                    {
                        document.Add(new Paragraph($"City: {location.City}, Street: {location.Street_Num}, Postal Code: {location.Postal_Code}"));
                    }

                    // Add order details
                    document.Add(new Paragraph("Order Details:"));
                    document.Add(new Paragraph($"Order ID: {order.OrderID}"));
                    document.Add(new Paragraph($"Order Date: {order.Order_Date}"));
                    document.Add(new Paragraph($"Specific Time: {order.SpecificTime}"));
                    document.Add(new Paragraph("Product Details:"));
                    foreach (var product in products)
                    {
                        document.Add(new Paragraph($"Product ID: {product.ProductID}, Product Name: {product.Name}, Price: {product.Price}, Quantity: {product.Quantity}"));
                    }

                    // Add service details to the PDF
                    document.Add(new Paragraph("Service Details:"));
                    foreach (var service in services)
                    {
                        document.Add(new Paragraph($"Service ID: {service.ServiceID}, Service Name: {service.Name}, Price: {service.Price}"));
                    }

                    // Add payment details
                    document.Add(new Paragraph("Payment Details:"));
                    document.Add(new Paragraph($"Payment Method: {payment.Method}"));
                }
            }

            return new PhysicalFileResult(pdfFilePath, "application/pdf");
        }

    }
}
