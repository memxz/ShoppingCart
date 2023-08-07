using ASPSession.Models;

namespace ASPSession.DAO.Interfaces
{
    public interface IRatingDB
    {
        public Rating GetAvgRating(int ProductId);
        public Dictionary<int, Rating> GetUserRatings(int CustomerId);
       /* public string GetStar(int pid);*/
        public bool AddStar(int cid, int pid, int rid);
        public bool UpdateStar(int cid, int pid, int rid);


    }
}
