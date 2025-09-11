namespace _3kmyung.Authentication.Domain
{
    public abstract record AuthenticationChannel
    {
        private AuthenticationChannel() { }

        public sealed record Provider : AuthenticationChannel
        {
            public IdentityProviderType ProviderType { get; }

            public Provider(IdentityProviderType providerType)
            {
                ProviderType = providerType;
            }
        }

        public sealed record CustomId : AuthenticationChannel { }

        public sealed record UsernameAndPassword : AuthenticationChannel { }

        public sealed record Anonymous : AuthenticationChannel { }
    }
}
