namespace Bps.uCloud.API
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.WindowsServices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implements the .Net Core ApiService as a Windows service.
    /// </summary>
    public class ApiService : WebHostService
    {
        #region Fields

        readonly ILogger<ApiService> logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Implements the .Net Core ApiService as a Windows service.
        /// </summary>
        /// <param name="host">The web host</param>
        public ApiService(IWebHost host) : base(host)
        {
            logger = host.Services.GetRequiredService<ILogger<ApiService>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes after ASP.NET Core starts.
        /// </summary>
        protected override void OnStarted()
        {
            //Startup.Instance.OnStarted();
            logger.LogTrace("{0} is running...", Program.AppName);
            base.OnStarted();
        }

        /// <summary>
        /// Executes after ASP.NET Core shuts down.
        /// </summary>
        protected override void OnStopped()
        {
            //Startup.Instance.OnStopped();
            logger.LogTrace("Stopped {0}. Good bye!", Program.AppName);
            base.OnStopped();
        }

        #endregion
    }
}
