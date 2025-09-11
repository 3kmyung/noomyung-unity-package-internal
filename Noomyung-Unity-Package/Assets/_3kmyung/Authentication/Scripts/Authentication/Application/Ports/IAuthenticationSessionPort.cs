using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Domain
{
    public interface IAuthenticationSessionPort : IDisposable
    {
        Task SignOutAsync(CancellationToken cancellationToken = default);

        Task<PlayerSession> GetAuthenticationSessionAsync(CancellationToken cancellationToken = default);
    }
}
