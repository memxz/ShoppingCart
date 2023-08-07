namespace ASPSession.DAO.Interfaces
{
    public interface ISecurityDB
    {

        public bool IdentifyUserDB(string Username, string Password, out string? customerID);
        public bool IsSessionValidDB(string customerID, string sessionID);
        public bool ProvideSessionDB(string customerID, string sessionID);
        public bool RemoveExpiredSessionDB(string customerID);

    }
}
