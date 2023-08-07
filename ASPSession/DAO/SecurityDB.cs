using System.Data;
using System.Text;
using System.Data.SqlClient;
using ASPSession.DAO.Interfaces;

namespace ASPSession.DAO
{
    public class SecurityDB : ISecurityDB
    {
        string connStr;
        public SecurityDB(IConfiguration cfg)
        {
            connStr = cfg.GetConnectionString("db_conn");
        }

        public bool IdentifyUserDB(string Username, string Password, out string? customerID) //returns true and customerID after matching Username/Password against the DB
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT customerID FROM Customers WHERE Username = @Username AND Password = @Password;";

                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar);
                    cmd.Parameters["@Username"].Value = Username;
                    cmd.Parameters["@Password"].Value = Password;
                    try
                    {

                        customerID = Convert.ToInt32(cmd.ExecuteScalar()).ToString(); //if it throws an exception means user not found, if user found return true and provide ID to calling method
                        return customerID != "0"; //since above expression ToInt32 converts null to 0, hence this check will make sure 0 is not returned as customerID

                    }
                    catch (Exception e) when (e is SqlException || e is NullReferenceException)
                    {
                        List<string> previous = File.ReadAllLines(@"Exceptions/Log.txt", Encoding.UTF8).ToList();
                        previous.Add(DateTime.Now.ToString("F"));
                        previous.Add(e.StackTrace!.ToString());
                        File.WriteAllLines(@"Exceptions/Log.txt", previous, Encoding.UTF8); ; //Logs stack trace into text file 
                        customerID = null; return false;
                    }
                }
            }
        }

        public bool IsSessionValidDB(string customerID, string sessionID) // check userid and sessionid, if user has session stored in DB, hence current login session is valid
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT sessionID FROM Sessions WHERE customerID = @customerID;";
                    cmd.Parameters.Add("@customerID", SqlDbType.Int);
                    cmd.Parameters["@customerID"].Value = Convert.ToInt32(customerID);

                    try
                    {
                        return cmd.ExecuteScalar().ToString() == sessionID; //checks if sessionID in DB matches cookiee sessionID

                    }
                    catch (Exception e) when (e is SqlException || e is NullReferenceException)
                    {
                        List<string> previous = File.ReadAllLines(@"Exceptions/Log.txt", Encoding.UTF8).ToList();
                        previous.Add(DateTime.Now.ToString("F"));
                        previous.Add(e.StackTrace!.ToString());
                        File.WriteAllLines(@"Exceptions/Log.txt", previous, Encoding.UTF8); ; //Logs stack trace into text file 
                        return false;
                    }

                }
            }
        }

        public bool ProvideSessionDB(string customerID, string sessionID) // Insert session in DB so that it can be used to authenticate session cookies
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO Sessions (customerID, sessionID) VALUES (@customerID, @sessionID);";
                    cmd.Parameters.Add("@customerID", SqlDbType.Int);
                    cmd.Parameters.Add("@sessionID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters["@customerID"].Value = Convert.ToInt32(customerID);
                    cmd.Parameters["@sessionID"].Value = Guid.Parse(sessionID);
                    try
                    {
                        return cmd.ExecuteNonQuery() == 1;  //will throw an exception if there is an old session, e.g., user could be logging in from another device

                    }
                    catch (SqlException e)
                    {
                        List<string> previous = File.ReadAllLines(@"Exceptions/Log.txt", Encoding.UTF8).ToList();
                        previous.Add(DateTime.Now.ToString("F"));
                        previous.Add(e.StackTrace!.ToString());
                        File.WriteAllLines(@"Exceptions/Log.txt", previous, Encoding.UTF8);
                        RemoveExpiredSessionDB(customerID);   // will remove old session
                    }
                    return ProvideSessionDB(customerID, sessionID); //and provide new session
                }
            }
        }

        public bool RemoveExpiredSessionDB(string customerID)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "DELETE FROM Sessions WHERE customerID = @customerID";
                    cmd.Parameters.Add("@customerID", SqlDbType.Int);
                    cmd.Parameters["@customerID"].Value = Convert.ToInt32(customerID);


                    try
                    {
                        return cmd.ExecuteNonQuery() == 1;    //returns true if a row is deleted

                    }
                    catch (SqlException e)
                    {
                        List<string> previous = File.ReadAllLines(@"Exceptions/Log.txt", Encoding.UTF8).ToList();
                        previous.Add(DateTime.Now.ToString("F"));
                        previous.Add(e.StackTrace!.ToString());
                        File.WriteAllLines(@"Exceptions/Log.txt", previous, Encoding.UTF8); ; //Logs stack trace into text file 
                        return false;
                    }

                }
            }
        }

    }
}
