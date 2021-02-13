namespace Bps.uCloud.Gateway.Settings
{
    public interface IKeyProvider
    {
        string Audience { get; }

        string Issuer { get; }

        byte[] SecretKey { get; }

        string SecretKeyBase64 { get; }
    }
}