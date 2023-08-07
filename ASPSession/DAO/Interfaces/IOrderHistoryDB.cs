using ASPSession.Models;
using ASPSession.Models.ViewModels;

namespace ASPSession.DAO.Interfaces
{
    public interface IOrderHistoryDB
    {
        public List<ActivationCode> GetActivationCodesByOrder(int orderID);
        public bool GetOrderByCustomer(string? customerID, ref List<Orders> orders);
        public List<Orders> GetOrderByCustomer(int customerID);
        public Product GetProductByID(int productID);




    }
}
