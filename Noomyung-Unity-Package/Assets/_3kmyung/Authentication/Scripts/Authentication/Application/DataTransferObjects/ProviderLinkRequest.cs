using System;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application.DataTransferObjects
{
    public sealed record ProviderLinkRequest
    {
        public IdentityProviderType ProviderType { get; }
        public string AccessToken { get; }

        public ProviderLinkRequest(IdentityProviderType providerType, string accessToken)
        {
            ProviderType = providerType;
            AccessToken = !string.IsNullOrWhiteSpace(accessToken) ? accessToken : throw new ArgumentException("Access token cannot be null or empty", nameof(accessToken));
        }
    }
}
