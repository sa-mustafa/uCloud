namespace Bps.uCloud.API
{
    using Bps.uCloud.API.Settings;
    using Microsoft.AspNetCore.Hosting;
    using System;

    /// <summary>
    /// Collection of extension functions
    /// </summary>
    public static class Extensions
    {
        //public static string GetDetailedError(this ModelValidationResult validationResult)
        //{
        //    var errors = validationResult.Errors
        //        .Select(x => new { Key = x.Key, Errors = x.Value.Select(y => y.ErrorMessage) })
        //        .Select(x => string.Format("Parameter = {0}, Errors = ({1})", x.Key, string.Join(", ", x.Errors)));

        //    return string.Format("Validation failed for Request Parameters: ({0})",
        //        string.Join(", ", errors));
        //}

        /// <summary>
        /// Runs the application as API service.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void RunApiService(this IWebHost host) =>
            System.ServiceProcess.ServiceBase.Run(new ApiService(host));

        /// <summary>
        /// Suggests queue name based on template and controller's name.
        /// </summary>
        /// <param name="cotroller">Specify the controller's name.</param>
        /// <param name="version">Specify the controller's version.</param>
        /// <param name="app">Set to application settings.</param>
        /// <returns>the suggested queue name.</returns>
        public static Uri SuggestQueueName(string cotroller, string version, IAppSettings app)
        {
            return new Uri(string.Format(app.QueueTemplate, cotroller, version));
        }

    }
}
