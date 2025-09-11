using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Defines the contract for user account deletion operations.
    /// </summary>
    public interface IAccountDeletionPort : IDisposable
    {
        Task DeleteAccountAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}
