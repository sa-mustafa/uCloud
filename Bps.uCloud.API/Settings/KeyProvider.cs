namespace Bps.uCloud.API.Settings
{
    using Microsoft.Extensions.Configuration;
    using System;

    public class KeyProvider : IKeyProvider
    {
        private Lazy<byte[]> secretKey;
        private readonly IConfiguration configuration;

        public KeyProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
            secretKey = new Lazy<byte[]>(() =>
            {
                return Convert.FromBase64String(SecretKeyBase64);
            });
        }

        public string Audience
        {
            get
            {
                return configuration["JWT:audience"];
            }
        }

        public string Issuer
        {
            get
            {
                return configuration["JWT:issuer"];
            }
        }

        public byte[] SecretKey
        {
            get
            {
                return secretKey.Value;
            }
        }

        public string SecretKeyBase64
        {
            get
            {
                return configuration["JWT:key"];
            }
        }

    }
}