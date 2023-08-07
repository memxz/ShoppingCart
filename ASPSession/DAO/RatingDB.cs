using ASPSession.DAO.Interfaces;
using ASPSession.Models;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace ASPSession.DAO
{
    public class RatingDB : IRatingDB
    {
        string connStr;
        public RatingDB(IConfiguration cfg)
        {
            connStr = cfg.GetConnectionString("db_conn");
        }

        public Rating GetAvgRating(int ProductId)
        {
            Rating rating = new Rating();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"select AVG(RatingValue) as AvgRating from rating where ProductId=" + ProductId;

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    rating.ProductId = ProductId;

                    if (reader["AvgRating"] == DBNull.Value)
                    {
                        rating.RatingValue = 0;
                    }
                    else
                    {
                        rating.RatingValue = (double)reader["AvgRating"];
                    }

                }
                conn.Close();

            }
            return rating;
        }

        public Dictionary<int, Rating> GetUserRatings(int CustomerId)
        {
            Dictionary<int, Rating> userRatingsDict = new Dictionary<int, Rating>();



            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sql = @"select * from rating where CustomerId=" + CustomerId;

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Rating rating = new Rating();
                    rating.RatingId = (int)reader["RatingId"];
                    rating.ProductId = (int)reader["ProductId"];
                    rating.CustomerId = (int)reader["CustomerId"];
                    rating.RatingValue = (int)reader["RatingValue"];
                    userRatingsDict.Add((int)reader["ProductId"], rating);

                }
                conn.Close();

            }
            return userRatingsDict;

        }





   /*     public string GetStar(int pid)
        {
            Star star = null;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string q = String.Format("SELECT * FROM Rating where CustomerId = 11 and ProductId = {0}", pid);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            star = new Star
                            {
                                UserId = reader.GetInt32(0),
                                ProductId = reader.GetInt32(1),
                                Rating = reader.GetInt32(2)
                            };
                        }
                    }
                }

                conn.Close();
            }


            return JsonSerializer.Serialize(star);

        }*/

        public bool AddStar(int cid, int pid, int rid)
        {
            bool status = false;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string q = String.Format("insert into Rating values ({0},{1}, {2})", cid, pid, rid);
                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        status = true;
                    }
                }
                conn.Close();
            }
            return status;
        }



        public bool UpdateStar(int cid, int pid, int rid)
        {
            bool status = false;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string q = String.Format(@"update Rating set ratingValue = {0} where CustomerId = {1} and ProductId = {2}", rid, cid, pid);
                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        status = true;
                    }
                }
                conn.Close();
            }
            return status;
        }

    }
}
