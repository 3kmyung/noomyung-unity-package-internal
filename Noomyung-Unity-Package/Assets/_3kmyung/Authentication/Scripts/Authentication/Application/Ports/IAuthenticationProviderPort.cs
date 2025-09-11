using System;
using System.Threading;
using System.Threading.Tasks;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Defines the contract for managing authentication provider linking and unlinking operations.
    /// </summary>
    public interface IProviderManagementPort : IDisposable
    {
        Task LinkProviderAsync(IdentityProviderType provider, string accessToken, CancellationToken cancellationToken = default);

        Task UnlinkProviderAsync(IdentityProviderType provider, CancellationToken cancellationToken = default);
    }
}
