using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Provider를 통한 로그인 Query
    /// </summary>
    public sealed class SignInWithProviderUseCase : IQuery<(SignInProviderType provider, string accessToken), string>
    {
        private readonly IAuthenticationPort _authenticationService;

        public SignInWithProviderUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<string> ExecuteAsync((SignInProviderType provider, string accessToken) request, CancellationToken cancellationToken = default)
        {
            var session = await _authenticationService.SignInWithProviderAsync(request.provider, request.accessToken, cancellationToken).ConfigureAwait(false);
            return session.PlayerID;
        }
    }
}
