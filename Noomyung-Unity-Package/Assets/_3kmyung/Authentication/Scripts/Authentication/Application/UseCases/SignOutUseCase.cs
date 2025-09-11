using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// 로그아웃 Command
    /// </summary>
    public sealed class SignOutUseCase : ICommand
    {
        private readonly IAuthenticationPort _authenticationService;

        public SignOutUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            await _authenticationService.SignOutAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
