using System.Data;
using System.Data.SqlClient;

namespace Project_test.Models
{
    public class DB
    {
        public string connectionString { get; set; }

        public DB()
        {
            connectionString = "Data Source=BAYOUMI; Initial Catalog=JOpera; Integrated Security=True;";
        }

        public DataTable? getUsers(Users user)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(connectionString);
            string query = "select * from Users";
            string query1 = $"insert into Users values {user.ID},{user.FName},{user.LName}";
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());

            }
            catch (SqlException err)
            {
                Console.WriteLine(err.Message);
                return null;
            }
            finally
            {
                con.Close();
            }
            return dt;
        }
    }
}
