namespace Bps.uCloud.Gateway.Settings
{
    using Bps.Common;
    using System;

    public interface IAppSettings
    {
        FileSize MaxUploadSize { get; }

        string RootPath { get; }

        Uri ServiceUri { get; }

        TimeSpan Timeout { get; }

        string UploadDirectory { get; }
    }
}
