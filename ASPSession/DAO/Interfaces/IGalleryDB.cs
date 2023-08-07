using ASPSession.Models;

namespace ASPSession.DAO.Interfaces
{
    public interface IGalleryDB
    {
        public List<Product> GetAllProducts();
        public List<Product> GetSearchProducts(string searchStr);
        public float GetTotalProducts();
    }
}
