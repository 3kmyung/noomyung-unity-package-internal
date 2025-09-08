using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// 플레이어 ID 조회 Query
    /// </summary>
    public sealed class GetPlayerIdUseCase : IQuery<string>
    {
        private readonly IAuthenticationPort _authenticationService;

        public GetPlayerIdUseCase(IAuthenticationPort authenticationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public async Task<string> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return await _authenticationService.GetPlayerIdAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
