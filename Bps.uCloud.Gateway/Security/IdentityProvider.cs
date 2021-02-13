namespace Bps.uCloud.Gateway.Security
{
    using Nancy;
    using Nancy.Authentication.Stateless;
    using System;
    using System.Security.Claims;

    internal class IdentityProvider : IIdentityProvider
    {
        private const string bearerHeader = "Bearer ";
        private readonly Settings.IKeyProvider keyProvider;

        public IdentityProvider(Settings.IKeyProvider KeyProvider)
        {
            keyProvider = KeyProvider;
        }

        public StatelessAuthenticationConfiguration Create()
        {
            return new StatelessAuthenticationConfiguration(GetUserFromIdentifier);
        }

        public ClaimsPrincipal/*IUserIdentity*/ GetUserFromIdentifier(NancyContext context)
        {
            try
            {
                var authHeader = context.Request.Headers.Authorization;
                if (IsTokenVoid(authHeader))
                    return null;

                var jwt = authHeader.Substring(bearerHeader.Length);
                var token = Jose.JWT.Decode<Token>(jwt, keyProvider.SecretKey, Jose.JwsAlgorithm.HS256);

                if (IsTokenIssuerInvalid(token))
                    return null;

                if (IsTokenExpired(token))
                    return null;

                return new ClaimsPrincipal(new UserIdentity(token));
            }
            catch
            {
                return null;
            }
        }

        private bool IsTokenIssuerInvalid(Token Token)
        {
            return !Token.Issuer.Equals(keyProvider.Issuer);
        }

        private bool IsTokenExpired(Token Token)
        {
            return Token.Expiry < DateTime.UtcNow;
        }

        private bool IsTokenVoid(string Token)
        {
            return string.IsNullOrWhiteSpace(Token);
        }

    }
}
