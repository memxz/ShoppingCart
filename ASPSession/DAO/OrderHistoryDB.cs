using ASPSession.DAO.Interfaces;
using ASPSession.Models;
using ASPSession.Models.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace ASPSession.DAO
{
    public class OrderHistoryDB : IOrderHistoryDB
    {
        string connStr;
        public OrderHistoryDB(IConfiguration cfg)
        {
            connStr = cfg.GetConnectionString("db_conn");
        }

        public List<Orders> GetOrderByCustomer(int customerID)
        {
            List<Orders> orders = new List<Orders>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string q = string.Format(@"SELECT * from Orders where CustomerId='{0}'", customerID);
                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Orders order = new Orders
                            {
                                OrderId = (int)reader["OrderId"],
                                ProductId = (int)reader["ProductId"],
                                OrderDate = (DateTime)reader["OrderDate"],
                                ProductQty = (int)reader["ProductQty"]
                            };
                            orders.Add(order);
                        }

                    }
                }
            }
            return orders;
        }

        public List<ActivationCode> GetActivationCodesByOrder(int orderID)
        {
            List<ActivationCode> codes = new List<ActivationCode>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT activationID FROM ActivationCodes where orderID= @orderID";
                    cmd.Parameters.Add("@orderID", SqlDbType.Int);
                    cmd.Parameters["@orderID"].Value = orderID;
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                ActivationCode code = new ActivationCode
                                {
                                    ActivationID = reader.GetGuid(0)
                                };
                                codes.Add(code);
                            }

                        }
                        catch (Exception e) when (e is SqlException || e is NullReferenceException)
                        {
                            List<string> previous = File.ReadAllLines(@"Exceptions/Log.txt", Encoding.UTF8).ToList();
                            previous.Add(DateTime.Now.ToString("F"));
                            previous.Add(e.StackTrace!.ToString());
                            File.WriteAllLines(@"Exceptions/Log.txt", previous, Encoding.UTF8); ; //Logs stack trace into text file 
                        }
                        return codes;
                    }
                }
            }
        }

        public Product GetProductByID(int productID)
        {
            Product product = new Product();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string q = string.Format(@"SELECT * FROM Product where ProductId='{0}'", productID);
                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            product = new Product
                            {
                                ProductId = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                ProductPrice = reader.GetDecimal(2),
                                ProductDesc = reader.GetString(3),
                                ProductIMG = reader.GetString(4)
                            };

                        }
                    }
                }
                conn.Close();
            }
            return product;
        }

   
        public bool GetOrderByCustomer(string? customerID, ref List<Orders> orders)
        {

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * from Orders where customerID= @customerID";
                    cmd.Parameters.Add("@customerID", SqlDbType.Int);
                    cmd.Parameters["@customerID"].Value = Convert.ToInt32(customerID);

                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Orders order = new Orders
                                {
                                    OrderId = reader.GetInt32(0),
                                    ProductId = reader.GetInt32(2),
                                    OrderDate = reader.GetDateTime(3),
                                    ProductQty = reader.GetInt32(4)
                                };
                                orders.Add(order);
                            }
                            return true;
                        }

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
       
    }
}