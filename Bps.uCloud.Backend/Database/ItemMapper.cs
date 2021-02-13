namespace Bps.uCloud.Backend.Database
{
    using Bps.uCloud.Contracts;
    using Bps.uCloud.Contracts.Entities;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;

    public class ItemMapper
    {
        private readonly IAerospikeClient DB;
        private readonly string Namespace;
        private const string setName = nameof(Item);
        private readonly WritePolicy writePolicy;

        public ItemMapper(IMongoDatabase DB, string Namespace)
        {
            this.DB = DB;
            this.Namespace = Namespace;
            writePolicy = new WritePolicy { recordExistsAction = RecordExistsAction.UPDATE };
        }

        public bool Clear(string galleryId)
        {
            // Scan items by gallery id.
            var query = new Statement()
            {
                //BinNames = ,
                Namespace = Namespace,
                SetName = setName,
                Filter = Filter.Equal("GalleryId", galleryId)
            };

            using (var set = DB.Query(null, query))
            {
                while (set.Next())
                    if (!DB.Delete(null, set.Key))
                        return false;
            }
            return true;
        }

        public List<Item> List(string galleryId)
        {
            // Inquires for items by gallery id.
            var result = new List<Item>();
            // Scan items by gallery id.
            var query = new Statement()
            {
                //BinNames = ,
                Namespace = Namespace,
                SetName = setName,
                Filter = Filter.Equal("GalleryId", galleryId)
            };

            using (var set = DB.Query(null, query))
            {
                while (set.Next())
                {
                    result.Add(new Item
                    {
                        GalleryId = set.Record.GetString("GalleryId"),
                        UserId = set.Record.GetString("UserId"),
                        Data = (byte[])set.Record.GetValue("Data"),
                        Name = set.Record.GetString("Name"),
                        Tag = set.Record.GetString("Tag"),
                        Timestamp = DateTime.FromBinary(set.Record.GetLong("Timestamp")),
                        Type = (DataTypes)set.Record.GetInt("Type")
                    });
                }
            }
            return result;
        }

        public void New(Item item)
        {
            // Creates a new gallery.
            var key = new Key(Namespace, setName, item.Id);
            Bin[] bins = {
                new Bin("GalleryId", item.GalleryId),
                new Bin("UserId", item.UserId),
                new Bin("Data", item.Data),
                new Bin("Name", item.Name),
                new Bin("Tag", item.Tag),
                new Bin("Timestamp", item.Timestamp.ToBinary()),
                new Bin("Type", (int)item.Type)
            };
            DB.Put(null, key, bins);
        }

        public Item View(string id)
        {
            // Inquires for an item by id.
            var key = new Key(Namespace, setName, id);
            Record record = DB.Get(null, key);
            if (record == null) return Item.Null;

            return new Item
            {
                GalleryId = record.GetString("GalleryId"),
                UserId = record.GetString("UserId"),
                Data = (byte[])record.GetValue("Data"),
                Name = record.GetString("Name"),
                Tag = record.GetString("Tag"),
                Timestamp = DateTime.FromBinary(record.GetLong("Timestamp")),
                Type = (DataTypes)record.GetInt("Type")
            };
        }

        public bool Remove(string id)
        {
            // Removes an item.
            var key = new Key(Namespace, setName, id);
            return DB.Delete(null, key);
        }

        public void Update(Item item)
        {
            // Creates or updates a gallery.
            var key = new Key(Namespace, setName, item.Id);
            Bin[] bins = {
                new Bin("GalleryId", item.GalleryId),
                new Bin("UserId", item.UserId),
                new Bin("Data", item.Data),
                new Bin("Name", item.Name),
                new Bin("Tag", item.Tag),
                new Bin("Timestamp", item.Timestamp.ToBinary()),
                new Bin("Type", (int)item.Type)
            };
            DB.Put(writePolicy, key, bins);
        }

    }
}