namespace Bps.uCloud.Backend
{
    using Bps.uCloud.Backend.Settings;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;

    public static class Program
    {
        #region Fields

        public static readonly string AppName = Common.Runtime.GetAppTitle();

        #endregion

        #region Methods

        public static void Main(string[] args)
        {
            Common.Exceptions.Setup("Bps.uCloud.Backend.NLog.config");

            // For more info, see '.NET Generic Host' on MSDN.
            var host = CreateHostBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    var config = ctx.Configuration;
                    services.AddHostedService<BackendService>();
                    services.AddLogging(logging =>
                    {
                        //logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Trace);
                        logging.AddNLog();
                    });
                    services.AddOptions<MongoDB>("MongoDB");
                    services.AddOptions<RabbitMQ>("RabbitMQ");
                    services.Configure<MongoDB>(config.GetSection("MongoDB"));
                    services.Configure<RabbitMQ>(config.GetSection("RabbitMQ"));
                })
                .UseConsoleLifetime()
                .UseWindowsService();

            host.Build().Run();

            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            Common.Exceptions.Shutdown();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);

        #endregion
    }
}
