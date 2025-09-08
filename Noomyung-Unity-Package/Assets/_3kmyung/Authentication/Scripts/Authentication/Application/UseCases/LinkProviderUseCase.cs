using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Provider 연결 Command
    /// </summary>
    public sealed class LinkProviderUseCase : ICommand<(AuthenticationProvider provider, string accessToken)>
    {
        private readonly IAuthenticationPort _authenticationService;

        public LinkProviderUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task ExecuteAsync((AuthenticationProvider provider, string accessToken) request, CancellationToken cancellationToken = default)
        {
            await _authenticationService.LinkProviderAsync(request.provider, request.accessToken, cancellationToken).ConfigureAwait(false);
        }
    }
}
