/*using System.Data;
using System.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace Project_test.Pages
{
    public class DB
    {
        *//*Data Source=MALAKELBANNA;Initial Catalog=JOperaFF;Integrated Security=True;Trust Server Certificate=True*//*
        public SqlConnection Con { get; set; }

        public DB()
        {
            string conStr = "Data Source=MALAKELBANNA;Initial Catalog=JOperaFF;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            Con = new SqlConnection(conStr);
        }
        public bool AddProduct(string tableName, string category, string name, int price, string description, List<IFormFile> images)
        {
            string insertProductQuery = "INSERT INTO " + tableName + " (Category, Name, Price, Description) VALUES (@Category, @Name, @Price, @Description); SELECT SCOPE_IDENTITY();";
            string insertProductImageQuery = "INSERT INTO ProductIMG (ProductID, img) VALUES (@ProductID, @ImageData)";

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

                    if (productId > 0)
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
                    else
                    {
                        return false;
                    }
                }
                Console.WriteLine("AddProduct method ended...");
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                Con.Close();
            }
        }

        public DataTable ReadInfo(string TableName, int UserID, string name, string email)
        {
            DataTable Table = new DataTable();
            string Q = "INSERT INTO Users (UserID, name, email) VALUES (@UserID, @name, @email)";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(Q, con);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                Table.Load(cmd.ExecuteReader());
            }
            catch (SqlException ex)
            {

            }
            finally { con.Close(); }
            return Table;
        }
        public DataTable SearchUser(string TableName, int UserID)
        {
            DataTable Table = new DataTable();
            string Q = "select name, email from " + TableName + " where UserID = @UserID";

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(Q, con);
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                Table.Load(cmd.ExecuteReader());
            }
            catch (SqlException ex)
            {

            }
            finally { con.Close(); }
            return Table;
        }
        public DataTable UpdateUser(string TableName, int UserID, string name, string email)
        {
            DataTable Table = new DataTable();
            string Q = "update " + TableName + " set name = @name, email = @email where UserID = @UserID";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(Q, con);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                Table.Load(cmd.ExecuteReader());
            }
            catch (SqlException ex)
            {

            }
            finally { con.Close(); }
            return Table;
        }
        public DataTable DeleteUser(string TableName, int UserID)
        {
            DataTable Table = new DataTable();
            string Q = "delete from " + TableName + " where UserID = @UserID";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(Q, con);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                Table.Load(cmd.ExecuteReader());
            }
            catch (SqlException ex)
            {

            }
            finally { con.Close(); }
            return Table;
        }
    }
}
*/