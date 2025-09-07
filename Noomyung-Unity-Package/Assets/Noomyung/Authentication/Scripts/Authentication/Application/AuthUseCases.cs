using System;
using System.Threading;
using System.Threading.Tasks;
using Noomyung.Authentication.Domain;

namespace Noomyung.Authentication.Application
{
    public sealed class AuthUseCases : IDisposable
    {
        private readonly IAuthService _authService;

        public AuthUseCases(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<string> SignInAnonymouslyAsync(CancellationToken cancellationToken = default)
        {
            var session = await _authService.SignInAnonymouslyAsync(cancellationToken).ConfigureAwait(false);

            return session.PlayerId;
        }

        public async Task<string> SignInWithProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            var session = await _authService.SignInWithProviderAsync(provider, accessToken, cancellationToken).ConfigureAwait(false);

            return session.PlayerId;
        }

        public async Task<string> SignInWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var session = await _authService.SignInWithUsernamePasswordAsync(username, password, cancellationToken).ConfigureAwait(false);

            return session.PlayerId;
        }

        public Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            return _authService.RegisterWithUsernamePasswordAsync(username, password, cancellationToken);
        }

        public Task LinkProviderAsync(AuthProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            return _authService.LinkProviderAsync(provider, accessToken, cancellationToken);
        }

        public Task UnlinkProviderAsync(AuthProvider provider, CancellationToken cancellationToken = default)
        {
            return _authService.UnlinkProviderAsync(provider, cancellationToken);
        }

        public Task<string> GetPlayerIdAsync(CancellationToken cancellationToken = default)
        {
            return _authService.GetPlayerIdAsync(cancellationToken);
        }

        public Task<bool> IsSignedInAsync(CancellationToken cancellationToken = default)
        {
            return _authService.IsSignedInAsync(cancellationToken);
        }

        public Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            return _authService.SignOutAsync(cancellationToken);
        }

        public void Dispose()
        {
            _authService.Dispose();
        }
    }
}
