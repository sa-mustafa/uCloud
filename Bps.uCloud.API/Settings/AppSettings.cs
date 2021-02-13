namespace Bps.uCloud.API.Settings
{
    using Bps.Common;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;

    /// <summary>
    /// Class where application settings are stored and shared.
    /// </summary>
    /// <seealso cref="IAppSettings" />
    public class AppSettings : IAppSettings
    {
        //public IBusControl Bus { get; }

        //public ISendEndpoint BusSend { get; }

        /// <summary>
        /// Gets the application queue template.
        /// </summary>
        public string QueueTemplate { get; }

        /// <summary>
        /// Gets the queue timeout.
        /// </summary>
        public TimeSpan QueueTimeout { get; }

        /// <summary>
        /// Gets the application root path.
        /// </summary>
        public string RootPath { get; }

        /// <summary>
        /// Gets the maximum size of the upload.
        /// </summary>
        public FileSize UploadMaxSize { get; }

        /// <summary>
        /// Gets the upload path where files are uploaded.
        /// </summary>
        public string UploadPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        /// <param name="Configuration">The application configuration.</param>
        public AppSettings(IConfiguration Configuration)
        {
            //Bus = Startup.Instance.Bus;

            QueueTemplate = $"{Configuration["RabbitMQ:uri"]}/{Configuration["RabbitMQ:queue:template"]}";

            QueueTimeout = TimeSpan.FromSeconds(int.Parse(Configuration["RabbitMQ:queue:timeout"]));

            RootPath = Runtime.GetAppDir();

            UploadMaxSize = FileSize.Create(10, FileSize.Unit.MegaByte);

            UploadPath = Path.Combine(RootPath, "Uploads");

            //throw new ArgumentNullException(nameof(serviceUri));
        }
    }
}
