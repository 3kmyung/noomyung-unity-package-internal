using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// 로그인 상태 확인 Query
    /// </summary>
    public sealed class IsSignedInUseCase : IQuery<bool>
    {
        private readonly IAuthenticationPort _authenticationService;

        public IsSignedInUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return await _authenticationService.IsSignedInAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
