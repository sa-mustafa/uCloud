namespace Bps.uCloud.Contracts.Entities
{
    using System;

    public interface IGallery
    {
        string Id { get; }

        string UserId { get; }

        //int Count { get; }

        string Name { get; }

        string Tag { get; }

        DateTime Timestamp { get; }

        bool Watchlist { get; }
    }
}
