namespace Bps.uCloud.API
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Web;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The class implementing the entry point of the application.
    /// </summary>
    public class Program
    {
        #region Fields

        /// <summary>
        /// The application name
        /// </summary>
        public static readonly string AppName = Common.Runtime.GetAppTitle();

        #endregion

        #region Methods

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("Bps.uCloud.API.NLog.config").GetCurrentClassLogger();
            //Common.Exceptions.Setup("Bps.uCloud.API.NLog.config");

            bool normal = Debugger.IsAttached || args.Contains("--console");
            // Alternative to TopShelf is Microsoft.AspNetCore.Hosting.WindowsServices (which needs full .NetFx).
            var builder = CreateHostBuilder(args);
            if (!normal)
            {
                var root = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                builder.UseContentRoot(root);
            }

            builder.ConfigureLogging(logging =>
            {
                //logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .UseNLog();

            var host = builder.Build();

            if (normal)
                host.Run();
            else
                host.RunApiService(); //RunAsService();

            host.WaitForShutdown();

            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            Common.Exceptions.Shutdown();
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>the host builder</returns>
        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        #endregion

    }
}
