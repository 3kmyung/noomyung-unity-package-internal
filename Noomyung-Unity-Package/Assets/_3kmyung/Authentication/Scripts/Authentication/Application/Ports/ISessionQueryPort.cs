using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Defines the contract for querying authentication session information.
    /// </summary>
    public interface ISessionQueryPort : IDisposable
    {
        Task<PlayerSession> GetAuthenticationSessionAsync(CancellationToken cancellationToken = default);
    }
}
