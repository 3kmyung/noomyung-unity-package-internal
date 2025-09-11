using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Defines the contract for user sign-in operations.
    /// </summary>
    public interface ISignInPort : IDisposable
    {
        Task<PlayerSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default);

        Task<PlayerSession> SignInWithProviderAsync(IdentityProviderType provider, string accessToken, CancellationToken cancellationToken = default);

        Task<PlayerSession> SignInWithUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}
