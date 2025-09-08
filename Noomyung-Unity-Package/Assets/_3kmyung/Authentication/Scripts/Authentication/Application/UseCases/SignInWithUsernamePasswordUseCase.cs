using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// 사용자명/비밀번호 로그인 Query
    /// </summary>
    public sealed class SignInWithUsernamePasswordUseCase : IQuery<(string username, string password), string>
    {
        private readonly IAuthenticationPort _authenticationService;

        public SignInWithUsernamePasswordUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<string> ExecuteAsync((string username, string password) request, CancellationToken cancellationToken = default)
        {
            var session = await _authenticationService.SignInWithUsernameAndPasswordAsync(request.username, request.password, cancellationToken).ConfigureAwait(false);
            return session.PlayerID;
        }
    }
}
