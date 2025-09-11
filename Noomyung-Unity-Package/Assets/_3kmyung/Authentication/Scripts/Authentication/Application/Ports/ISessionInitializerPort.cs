using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Defines the contract for initializing authentication sessions.
    /// </summary>
    public interface ISessionInitializerPort : IDisposable
    {
        Task InitializeAnonymousSessionAsync(CancellationToken cancellationToken = default);
    }
}
