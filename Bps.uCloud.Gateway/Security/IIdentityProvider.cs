namespace Bps.uCloud.Gateway.Security
{
    using Nancy;
    using Nancy.Authentication.Stateless;
    using System.Security.Claims;

    internal interface IIdentityProvider
    {
        StatelessAuthenticationConfiguration Create();
        ClaimsPrincipal/*IUserIdentity*/ GetUserFromIdentifier(NancyContext context);
    }
}