namespace Bps.uCloud.Contracts
{
    using System.Runtime.Serialization;

    public enum ImageTypes
    {
        [EnumMember(Value = "url")]
        Url = 0,
        [EnumMember(Value = "base64")]
        Base64 = 1
    }
}
