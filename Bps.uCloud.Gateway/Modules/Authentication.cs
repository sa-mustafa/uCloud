namespace Bps.uCloud.Gateway.Modules
{
    using Jose;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Validation;
    using Security;
    using System;

    public class Authentication : NancyModule
    {
        public Authentication(Settings.IKeyProvider keyProvider)
        {
            Get("/values", _ => Response.AsJson(new string[] { "value1", "value2" }));

            Get("/login", _ =>
            {
                // It should return the view that contains the login form.
                return View["login"];
            });

            Post("/login", _ =>
            {
                // It should validate the user based on posted data and perform 
                // one of the Login actions.
                var login = this.Bind<Requests.Login>();
                var result = this.Validate(login);
                if (!result.IsValid)
                {
                    ViewBag.ValidationError = result.GetDetailedError();
                    return View["login"];
                }

                // TODO: Authenticate the user and return a token.
                DateTime issuedAt = DateTime.UtcNow;
                DateTime expiry = issuedAt.AddMinutes(2);
                var payload = new Token()
                {
                    Id = Guid.NewGuid(),
                    Issuer = keyProvider.Issuer,
                    Expiry = expiry,
                    Name = login.UserName
                };
                /*var payload = new Dictionary<string, object>()
                {
                    { "aud", KeyProvider.Audience},     // Audience
                    { "iss", KeyProvider.Issuer },      // Issuer
                    { "sub", "" },                      // Subject
                    { "jti", Guid.NewGuid().ToString()},// JWT ID
                    { "iat", issuedAt.ToString() },     // Issued At
                    { "exp", expiry.ToString() }        // Expiration
                }*/
                string jwt = JWT.Encode(payload, keyProvider.SecretKey, JwsAlgorithm.HS256);
                var response = new { token = jwt };
                return Response.AsJson(response);
            });
        }
    }
}