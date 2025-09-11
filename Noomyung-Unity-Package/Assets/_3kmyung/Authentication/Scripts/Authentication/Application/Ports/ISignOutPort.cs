using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Defines the contract for user sign-out operations.
    /// </summary>
    public interface ISignOutPort : IDisposable
    {
        Task SignOutAsync(CancellationToken cancellationToken = default);
    }
}
