namespace Bps.uCloud.API.Settings
{
    using Bps.Common;
    using MassTransit;
    using System;

    /// <summary>
    /// Application Settings
    /// </summary>
    public interface IAppSettings
    {
        //IBusControl Bus { get; }

        //ISendEndpoint BusSend { get; }

        /// <summary>
        /// Gets the application queue template.
        /// </summary>
        string QueueTemplate { get; }

        /// <summary>
        /// Gets the application queue timeout.
        /// </summary>
        TimeSpan QueueTimeout { get; }

        /// <summary>
        /// Gets the application root path.
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// Gets the maximum size of the upload.
        /// </summary>
        FileSize UploadMaxSize { get; }

        /// <summary>
        /// Gets the upload path where files are uploaded.
        /// </summary>
        string UploadPath { get; }
    }
}
