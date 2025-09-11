using System;
using System.Threading;
using System.Threading.Tasks;

namespace _3kmyung.Authentication.Domain
{
    public interface IAuthenticationProviderPort : IDisposable
    {
        Task LinkProviderAsync(SignInProviderType provider, string accessToken, CancellationToken cancellationToken = default);

        Task UnlinkProviderAsync(SignInProviderType provider, CancellationToken cancellationToken = default);
    }
}
