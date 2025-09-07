using System;
using System.Threading;
using System.Threading.Tasks;

namespace Noomyung.Authentication.Domain
{
    public enum AuthProvider
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

    public interface IAuthService : IDisposable
    {
        Task<IAuthSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default);

        Task<IAuthSession> SignInWithDeviceID(string deviceID, CancellationToken cancellationToken = default);

        Task<IAuthSession> SignInWithProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default);

        Task<IAuthSession> SignInWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task LinkProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default);

        Task UnlinkProviderAsync(AuthProvider provider, CancellationToken cancellationToken = default);

        Task<string> GetPlayerIdAsync(CancellationToken cancellationToken = default);

        Task SignOutAsync(CancellationToken cancellationToken = default);
    }
}
