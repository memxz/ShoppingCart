using Microsoft.Data.SqlClient;
using ASPSession.Models.ViewModels;
using System.Data;
using System.Text;
using ASPSession.Models;
using ASPSession.DAO.Interfaces;

namespace ASPSession.DAO
{
    public class CartDB : ICartDB
    {
        string connStr;
        IOrderHistoryDB odb;
        public CartDB(IConfiguration cfg, IOrderHistoryDB odb)
        {
            connStr = cfg.GetConnectionString("db_conn");
            this.odb = odb;
        }

        public int AddOrderById(List<CartItem> items, int custID)
        {
            if (items.Count == 0 || custID == 0)
            {
                return 0;
            }
            int res = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO Orders (CustomerId, ProductId, ProductQty) VALUES (@CustomerId,@ProductId,@ProductQty)";
                    cmd.Parameters.Add("@CustomerId", SqlDbType.Int);
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int);
                    cmd.Parameters.Add("@ProductQty", SqlDbType.Int);
                    foreach (CartItem item in items)
                    {

                        cmd.Parameters["@CustomerId"].Value = custID;
                        cmd.Parameters["@ProductId"].Value = item.ProductId;
                        cmd.Parameters["@ProductQty"].Value = item.Quantity;

                        try
                        {
                            res = cmd.ExecuteNonQuery();
                        }
                        catch (Exception e) when (e is SqlException || e is NullReferenceException)
                        {
                            List<string> previous = File.ReadAllLines(@"Exceptions/Log.txt", Encoding.UTF8).ToList();
                            previous.Add(DateTime.Now.ToString("F"));
                            previous.Add(e.StackTrace!.ToString());
                            File.WriteAllLines(@"Exceptions/Log.txt", previous, Encoding.UTF8);

                        }
                    }
                }

            }
            return res;
        }

        public int RemoveCartById(int custID)
        {
            int noAffectedRowsconn = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string q = string.Format(@"DELETE FROM Cart WHERE CustomerId='{0}'", custID);
                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    noAffectedRowsconn = cmd.ExecuteNonQuery();

                }

                conn.Close();
            }
            return noAffectedRowsconn;
        }


        public bool AddActivationCode(int custID)
        {

            List<Orders> listOrder = odb.GetOrderByCustomer(custID);
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO ActivationCodes (ActivationID,OrderId) VALUES (@ActivationID,@OrderId)";
                    cmd.Parameters.Add("@ActivationID", SqlDbType.UniqueIdentifier);
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int);
                    foreach (Orders od in listOrder)
                    {
                        for (int i = 0; i < od.ProductQty; ++i)
                        {
                            cmd.Parameters["@ActivationID"].Value = Guid.NewGuid();
                            cmd.Parameters["@OrderId"].Value = od.OrderId;

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e) when (e is SqlException || e is NullReferenceException)
                            {
                                List<string> previous = File.ReadAllLines(@"Exceptions/Log.txt", Encoding.UTF8).ToList();
                                previous.Add(DateTime.Now.ToString("F"));
                                previous.Add(e.StackTrace!.ToString());
                                File.WriteAllLines(@"Exceptions/Log.txt", previous, Encoding.UTF8);
                                return false;
                            }
                        }

                    }
                }

            }
            return true;
        }
    }
}
