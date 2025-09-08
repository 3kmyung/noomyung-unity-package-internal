using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Domain
{
    public interface IAuthenticationPort : IDisposable
    {
        Task<IAuthenticationSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default);

        Task<IAuthenticationSession> SignInWithProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default);

        Task<IAuthenticationSession> SignInWithUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task SignOutAsync(CancellationToken cancellationToken = default);

        Task LinkProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default);

        Task UnlinkProviderAsync(AuthenticationProvider provider, CancellationToken cancellationToken = default);

        Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task UnregisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task<IAuthenticationSession> GetAuthenticationSessionAsync(CancellationToken cancellationToken = default);
    }
}

