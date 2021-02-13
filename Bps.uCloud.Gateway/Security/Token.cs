namespace Bps.uCloud.Gateway.Security
{
    using System;

    sealed class Token
    {
        public Guid Id { get; set; }
        public string Issuer { get; set; }
        public string Name { get; set; }
        public DateTime Expiry { get; set; }
    }
}
