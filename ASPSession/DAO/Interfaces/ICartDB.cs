using ASPSession.Models.ViewModels;

namespace ASPSession.DAO.Interfaces
{
    public interface ICartDB
    {
        public int AddOrderById(List<CartItem> items, int custID);
        public int RemoveCartById(int custID);
        public bool AddActivationCode(int custID);
    }
}
