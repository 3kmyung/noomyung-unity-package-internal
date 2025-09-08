using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// 사용자명/비밀번호 등록 Command
    /// </summary>
    public sealed class RegisterWithUsernamePasswordUseCase : ICommand<(string username, string password)>
    {
        private readonly IAuthenticationPort _authenticationService;

        public RegisterWithUsernamePasswordUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task ExecuteAsync((string username, string password) request, CancellationToken cancellationToken = default)
        {
            await _authenticationService.RegisterWithUsernamePasswordAsync(request.username, request.password, cancellationToken).ConfigureAwait(false);
        }
    }
}
