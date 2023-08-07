using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using ASPSession.Models;
using ASPSession.DAO.Interfaces;

namespace ASPSession.DAO;

public class GalleryDB : IGalleryDB 
{
    string connStr;
    public GalleryDB(IConfiguration cfg)
    {
        connStr = cfg.GetConnectionString("db_conn");
    }

   
 public List<Product> GetAllProducts()
        {
            List<Product> product = new List<Product>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sql = @"SELECT ProductId, ProductName, productPrice, productDesc, productIMG
                            FROM Product";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product p = new Product()
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        ProductPrice = (decimal)reader["productPrice"],
                        ProductDesc = (string)reader["productDesc"],
                        ProductIMG = (string)reader["productIMG"]
                    };
                    product.Add(p);//add every single product to the product LIST by looping.
                }
            }
            return product;
        }


    public List<Product> GetSearchProducts(string searchStr)
    {
        List<Product> product = new List<Product>();
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            string sql = @"SELECT ProductId, ProductName, productPrice, productDesc, productIMG
                            FROM Product where ProductName like ('%" + searchStr + "%') or productDesc like ('%" + searchStr + "%') ";

            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Product p = new Product()
                {
                    ProductId = (int)reader["ProductId"],
                    ProductName = (string)reader["ProductName"],
                    ProductPrice = (decimal)reader["productPrice"],
                    ProductDesc = (string)reader["productDesc"],
                    ProductIMG = (string)reader["productIMG"]
                };
                product.Add(p);
            }
        }
        return product;
    }

    public float GetTotalProducts()
    {
        float totalProducts = 0;


        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            string sql = @"SELECT COUNT(productid) AS productCount from product";

            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                totalProducts = (int)reader["productCount"];
            }
            conn.Close();

        }
        return totalProducts;
    }


}
