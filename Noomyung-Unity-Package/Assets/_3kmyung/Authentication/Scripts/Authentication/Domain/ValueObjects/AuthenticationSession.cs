using System;

namespace _3kmyung.Authentication.Domain
{
    public sealed record AuthenticationSession
    {
        public AuthenticationChannel Channel { get; }

        public DateTimeOffset SignedInAt { get; }

        public DateTimeOffset ExpiresAt { get; }

        public AuthenticationSession(AuthenticationChannel channel, DateTimeOffset signedInAt, DateTimeOffset expiresAt)
        {
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            SignedInAt = signedInAt != default ? signedInAt : throw new ArgumentException("Sign in time must be specified.", nameof(signedInAt));
            ExpiresAt = expiresAt != default ? expiresAt : throw new ArgumentException("Expire time must be specified.", nameof(expiresAt));
        }

        public bool IsExpired(DateTimeOffset nowUtc) => nowUtc.ToUniversalTime() >= ExpiresAt;
    }
}
