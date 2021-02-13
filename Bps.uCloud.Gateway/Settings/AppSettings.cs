namespace Bps.uCloud.Gateway.Settings
{
    using Bps.Common;
    using System;

    public class AppSettings : IAppSettings
    {
        public FileSize MaxUploadSize { get; }

        public string RootPath { get; }

        public string UploadDirectory { get; }

        public Uri ServiceUri { get; }

        public TimeSpan Timeout { get; }

        public AppSettings(FileSize maxUploadSize, string rootPath, string serviceUri, int timeout, string uploadDirectory)
        {
            if (maxUploadSize == null)
                throw new ArgumentNullException(nameof(maxUploadSize));
            MaxUploadSize = maxUploadSize;

            if (rootPath == null)
                throw new ArgumentNullException(nameof(rootPath));
            RootPath = rootPath;

            if (serviceUri == null)
                throw new ArgumentNullException(nameof(serviceUri));
            ServiceUri = new Uri(serviceUri);

            Timeout = TimeSpan.FromSeconds(timeout);

            if (uploadDirectory == null)
                throw new ArgumentNullException(nameof(maxUploadSize));
            UploadDirectory = uploadDirectory;
        }
    }
}
