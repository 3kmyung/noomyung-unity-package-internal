namespace _3kmyung.Authentication.Domain
{
    public abstract record SignInChannel
    {
        private SignInChannel() { }

        public sealed record Provider : SignInChannel
        {
            public SignInProviderType ProviderType { get; }

            public Provider(SignInProviderType providerType)
            {
                ProviderType = providerType;
            }
        }

        public sealed record CustomId : SignInChannel { }

        public sealed record UsernameAndPassword : SignInChannel { }

        public sealed record Anonymous : SignInChannel { }
    }
}
