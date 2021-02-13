namespace Bps.uCloud.Gateway
{
    using Microsoft.AspNetCore.Hosting;
    using Nancy.Validation;
    using System.Linq;

    public static class Extensions
    {
        public static string GetDetailedError(this ModelValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .Select(x => new { Key = x.Key, Errors = x.Value.Select(y => y.ErrorMessage) })
                .Select(x => string.Format("Parameter = {0}, Errors = ({1})", x.Key, string.Join(", ", x.Errors)));

            return string.Format("Validation failed for Request Parameters: ({0})",
                string.Join(", ", errors));
        }

        public static void RunGatewayService(this IWebHost host) =>
            System.ServiceProcess.ServiceBase.Run(new GatewayService(host));
    }

}
