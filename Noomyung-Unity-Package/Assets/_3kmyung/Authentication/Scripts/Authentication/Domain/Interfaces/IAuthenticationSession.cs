namespace _3kmyung.Authentication.Domain
{
    public interface IAuthenticationSession
    {
        string PlayerGUID { get; }

        bool IsSignedIn { get; }
    }
}
