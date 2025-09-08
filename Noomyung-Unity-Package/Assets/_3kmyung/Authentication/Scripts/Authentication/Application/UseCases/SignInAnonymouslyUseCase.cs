using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// 익명 로그인 Query
    /// </summary>
    public sealed class SignInAnonymouslyUseCase : IQuery<string>
    {
        private readonly IAuthenticationPort _authenticationService;

        public SignInAnonymouslyUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<string> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var session = await _authenticationService.SignInAnonymouslyAsync(cancellationToken).ConfigureAwait(false);
            return session.PlayerID;
        }
    }
}
