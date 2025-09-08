using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    public sealed class AuthenticationUseCases : IDisposable
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationUseCases(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<string> SignInAnonymouslyAsync(CancellationToken cancellationToken = default)
        {
            var session = await _authenticationService.SignInAnonymouslyAsync(cancellationToken).ConfigureAwait(false);

            return session.PlayerId;
        }

        public async Task<string> SignInWithProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            var session = await _authenticationService.SignInWithProviderAsync(provider, accessToken, cancellationToken).ConfigureAwait(false);

            return session.PlayerId;
        }

        public async Task<string> SignInWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var session = await _authenticationService.SignInWithUsernamePasswordAsync(username, password, cancellationToken).ConfigureAwait(false);

            return session.PlayerId;
        }

        public Task RegisterWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            return _authenticationService.RegisterWithUsernamePasswordAsync(username, password, cancellationToken);
        }

        public Task LinkProviderAsync(AuthenticationProvider provider, string accessToken, CancellationToken cancellationToken = default)
        {
            return _authenticationService.LinkProviderAsync(provider, accessToken, cancellationToken);
        }

        public Task UnlinkProviderAsync(AuthenticationProvider provider, CancellationToken cancellationToken = default)
        {
            return _authenticationService.UnlinkProviderAsync(provider, cancellationToken);
        }

        public Task<string> GetPlayerIdAsync(CancellationToken cancellationToken = default)
        {
            return _authenticationService.GetPlayerIdAsync(cancellationToken);
        }

        public Task<bool> IsSignedInAsync(CancellationToken cancellationToken = default)
        {
            return _authenticationService.IsSignedInAsync(cancellationToken);
        }

        public Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            return _authenticationService.SignOutAsync(cancellationToken);
        }

        public void Dispose()
        {
            _authenticationService.Dispose();
        }
    }
}

