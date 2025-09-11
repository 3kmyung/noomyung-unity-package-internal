using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Domain
{
    public interface IAccountManagementPort : IDisposable
    {
        Task DeleteAccountAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}
