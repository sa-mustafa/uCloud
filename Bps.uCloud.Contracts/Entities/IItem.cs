namespace Bps.uCloud.Contracts.Entities
{
    using System;

    public interface IItem
    {
        string Id { get; }

        string GalleryId { get; }

        string UserId { get; }

        byte[] Data { get; }

        string Name { get; }

        Operations Operation { get; }

        string Tag { get; }

        DateTime Timestamp { get; }

        DataTypes Type { get; }
    }
}
