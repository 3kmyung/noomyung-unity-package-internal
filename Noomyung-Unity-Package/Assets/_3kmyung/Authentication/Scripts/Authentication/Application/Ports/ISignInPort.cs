using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Domain
{
    public interface ISignInPort : IDisposable
    {
        Task<PlayerSession> SignInAnonymouslyAsync(CancellationToken cancellationToken = default);

        Task<PlayerSession> SignInWithProviderAsync(SignInProviderType provider, string accessToken, CancellationToken cancellationToken = default);

        Task<PlayerSession> SignInWithUsernameAndPasswordAsync(string username, string password, CancellationToken cancellationToken = default);

        Task InitializeAnonymousSessionAsync(CancellationToken cancellationToken = default);
    }
}
