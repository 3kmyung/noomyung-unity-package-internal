using System;

namespace _3kmyung.Authentication.Domain
{
    public record PlayerSession
    {
        public Guid Id { get; }

        public string PlayerId { get; }

        public SignInRecord LastSignIn { get; }

        public PlayerSession(Guid id, string playerId, SignInRecord lastSignIn)
        {
            Id = id;
            PlayerId = playerId ?? throw new ArgumentNullException(nameof(playerId));
            LastSignIn = lastSignIn ?? throw new ArgumentException(nameof(lastSignIn));
        }

        public bool IsActive(DateTimeOffset nowUtc) => !LastSignIn.IsExpired(nowUtc);
    }
}
