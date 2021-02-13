namespace Bps.uCloud.Gateway
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public static class Program
    {
        #region Fields

        public static readonly string AppName = Common.Runtime.GetAppTitle();

        #endregion

        #region Methods

        public static void Main(string[] args)
        {
            Common.Exceptions.Setup("Bps.uCloud.Gateway.NLog.config");

            bool normal = Debugger.IsAttached || args.Contains("--console");
            // Alternative to TopShelf is Microsoft.AspNetCore.Hosting.WindowsServices (which needs full .NetFx).
            var builder = CreateWebHostBuilder(args);

            if (!normal)
                builder.UseContentRoot
                    (Path.GetDirectoryName
                        (Process.GetCurrentProcess().MainModule.FileName));

            var host = builder.Build();
            
            if (normal)
                host.Run();
            else
                host.RunGatewayService();

            host.WaitForShutdown();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>();

        #endregion
    }
}
