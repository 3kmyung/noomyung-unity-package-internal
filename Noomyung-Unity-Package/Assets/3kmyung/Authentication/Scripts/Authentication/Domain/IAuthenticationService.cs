using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Domain
{
    public enum AuthenticationProvider
    {
        Anonymous,
        Google,
        Apple,
        Facebook,
        Steam,
        Custom,
        UsernamePassword
    }

    public interface IAuthSession
    {
        string PlayerId { get; }

        bool IsSignedIn { get; }
    }

    public interface IAuthenticationService : IDisposable
    {
        Task<IAuthSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default);

        Task<IAuthSession> SignInWithDeviceID(string deviceID, CancellationToken cancellationToken = default);

        Task<IAuthSession> SignInWithProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default);

        Task<IAuthSession> SignInWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task LinkProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default);

        Task UnlinkProviderAsync(AuthenticationProvider provider, CancellationToken cancellationToken = default);

        Task<string> GetPlayerIdAsync(CancellationToken cancellationToken = default);

        Task<bool> IsSignedInAsync(CancellationToken cancellationToken = default);

        Task SignOutAsync(CancellationToken cancellationToken = default);
    }
}

