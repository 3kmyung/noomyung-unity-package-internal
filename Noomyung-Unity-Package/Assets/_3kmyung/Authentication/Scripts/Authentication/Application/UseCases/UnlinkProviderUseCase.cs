using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Provider 연결 해제 Command
    /// </summary>
    public sealed class UnlinkProviderUseCase : ICommand<IdentityProviderType>
    {
        private readonly IAuthenticationPort _authenticationService;

        public UnlinkProviderUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task ExecuteAsync(IdentityProviderType request, CancellationToken cancellationToken = default)
        {
            await _authenticationService.UnlinkProviderAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
