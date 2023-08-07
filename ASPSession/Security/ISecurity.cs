namespace ASPSession.Security
{
    public interface ISecurity
    {
        public bool LoginAuthentication(string Username, string Password, out string? customerID);

        public bool Authenticate();
        public bool RemoveAuthentication();
        public void ProvideSession(string Username, string customerID);
    }
}
