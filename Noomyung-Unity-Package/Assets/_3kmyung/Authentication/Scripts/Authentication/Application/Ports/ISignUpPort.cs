using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Defines the contract for user registration operations.
    /// </summary>
    public interface ISignUpPort : IDisposable
    {
        Task SignUpWithUsernamePasswordAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}