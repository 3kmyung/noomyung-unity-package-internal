namespace _3kmyung.Authentication.Domain
{
    public interface IAuthenticationSession
    {
        string PlayerID { get; }

        bool IsSignedIn { get; }
    }
}
