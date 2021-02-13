namespace Bps.uCloud.Contracts.Entities
{
    using MongoDB.Bson.Serialization.Attributes;
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    public class Item : IItem
    {
        [BsonId]
        [IgnoreDataMember]
        public string dbId { get; set; }

        public string Id
        {
            get => GenerateID(GalleryId, Name);
        }

        public string GalleryId { get; set; }

        public string UserId { get; set; }

        public byte[] Data { get; set; }

        public string Name { get; set; }

        public Operations Operation { get; set; }

        public string Tag { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual DataTypes Type { get; set; }

        public Item()
        {
            Timestamp = DateTime.UtcNow;
        }

        public Item(Image image)
        {
            if (image.Type == ImageTypes.Url)
            {
                using (var wc = new WebClient())
                {
                    Data = wc.DownloadData(image.Source);
                }
            }
            else
            {
                Data = Convert.FromBase64String(image.Source);
            }

            Name = image.Name;
            Tag = image.Tag;
            Timestamp = DateTime.UtcNow;
            Type = DataTypes.Image;
        }

        public Item(IItem item)
        {
            GalleryId = item.GalleryId;
            UserId = item.UserId;
            Data = item.Data;
            Name = item.Name;
            Operation = item.Operation;
            Tag = item.Tag;
            Timestamp = item.Timestamp;
            Type = item.Type;
        }

        public static string GenerateID(string GalleryId, string Name)
        {
            return GalleryId + ":" + Name;
        }

        public bool IsNull => Id == ":";

        public static Item Null => new Item();

        public static EntityRequest<Item> Request(Tasks type, Item request)
        {
            return new EntityRequest<Item>(request, type);
        }

        public static EntityRequest<Item> Request(Tasks type, string userId)
        {
            return new EntityRequest<Item>(new Item() { UserId = userId }, type);
        }

        public static EntityRequest<Item> Request(Tasks type, string userId, string galleryName)
        {
            return new EntityRequest<Item>(new Item()
            {
                UserId = userId,
                GalleryId = Gallery.GenerateID(userId, galleryName)
            }, type);
        }

        public static EntityRequest<Item> Request(Tasks type, string userId, string galleryName, string name)
        {
            return new EntityRequest<Item>(new Item()
            {
                UserId = userId,
                GalleryId = Gallery.GenerateID(userId, galleryName),
                Name = name
            }, type);
        }

        public static EntityResponse<Item> Response()
        {
            return new EntityResponse<Item>();
        }

    }
}
