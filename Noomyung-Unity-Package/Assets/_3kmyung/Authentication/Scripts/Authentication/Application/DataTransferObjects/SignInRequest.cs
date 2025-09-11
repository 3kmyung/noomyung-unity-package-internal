using System;
using _3kmyung.Authentication.Domain;

namespace _3kmyung.Authentication.Application.DataTransferObjects
{
    public sealed record SignInRequest
    {
        public SignInChannel Channel { get; }
        public string? Username { get; }
        public string? Password { get; }
        public SignInProviderType? ProviderType { get; }
        public string? AccessToken { get; }

        public SignInRequest(SignInChannel channel, string? username = null, string? password = null, SignInProviderType? providerType = null, string? accessToken = null)
        {
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Username = username;
            Password = password;
            ProviderType = providerType;
            AccessToken = accessToken;
        }
    }
}
