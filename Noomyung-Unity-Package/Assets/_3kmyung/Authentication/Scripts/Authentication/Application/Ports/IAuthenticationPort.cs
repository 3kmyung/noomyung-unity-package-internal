using System;

namespace _3kmyung.Authentication.Application
{
    /// <summary>
    /// Composite interface that combines all authentication-related ports.
    /// Maintains backward compatibility while allowing for more granular port separation.
    /// </summary>
    public interface IAuthenticationPort : ISignInPort, ISessionInitializerPort, IProviderManagementPort, ISignUpPort, IAccountDeletionPort, ISignOutPort, ISessionQueryPort
    {
        // This interface inherits all methods from the following specialized ports:
        // - ISignInPort: SignInAnonymouslyAsync, SignInWithProviderAsync, SignInWithUsernameAndPasswordAsync
        // - ISessionInitializerPort: InitializeAnonymousSessionAsync
        // - IProviderManagementPort: LinkProviderAsync, UnlinkProviderAsync
        // - ISignUpPort: SignUpWithUsernamePasswordAsync
        // - IAccountDeletionPort: DeleteAccountAsync
        // - ISignOutPort: SignOutAsync
        // - ISessionQueryPort: GetAuthenticationSessionAsync
    }
}
