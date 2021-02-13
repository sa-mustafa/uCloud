namespace Bps.uCloud.Backend.Database
{
    using Aerospike.Client;
    using Bps.uCloud.Contracts.Entities;
    using System;
    using System.Collections.Generic;

    public class GalleryMapper
    {
        private readonly IAerospikeClient DB;
        private readonly string Namespace;
        private const string setName = "Gallery";
        private readonly WritePolicy writePolicy;

        public GalleryMapper(IAerospikeClient DB, string Namespace)
        {
            this.DB = DB;
            this.Namespace = Namespace;
            writePolicy = new WritePolicy();
            writePolicy.recordExistsAction = RecordExistsAction.UPDATE;
        }

        public bool Clear(string userId)
        {
            // Scan items by gallery id.
            var query = new Statement()
            {
                //BinNames = ,
                Namespace = Namespace,
                SetName = setName,
                Filter = Filter.Equal("UserId", userId)
            };

            using (var set = DB.Query(null, query))
            {
                while (set.Next())
                    if (!DB.Delete(null, set.Key))
                        return false;
            }
            return true;
        }

        public List<Gallery> List(string userId)
        {
            // Inquires for galleries by user id.
            var result = new List<Gallery>();
            // Scan galleries by user id.
            var query = new Statement()
            {
                //BinNames = ,
                Namespace = Namespace,
                SetName = setName,
                Filter = Filter.Equal("UserId", userId)
            };

            using (var set = DB.Query(null, query))
            {
                while (set.Next())
                {
                    result.Add(new Gallery
                    {
                        UserId = set.Record.GetString("UserId"),
                        Name = set.Record.GetString("Name"),
                        Tag = set.Record.GetString("Tag"),
                        Timestamp = DateTime.FromBinary(set.Record.GetLong("Timestamp"))
                    });
                }
            }
            return result;
        }

        public void New(Gallery gallery)
        {
            // Creates a new gallery.
            var key = new Key(Namespace, setName, gallery.Id);
            Bin[] bins = {
                new Bin("UserId", gallery.UserId),
                new Bin("Name", gallery.Name),
                new Bin("Tag", gallery.Tag),
                new Bin("Timestamp", gallery.Timestamp.ToBinary())
            };
            DB.Put(null, key, bins);
        }

        public Gallery View(string id)
        {
            // Inquires for a gallery by id.
            var key = new Key(Namespace, setName, id);
            Record record = DB.Get(null, key);
            if (record == null) return Gallery.Null;

            return new Gallery
            {
                UserId = record.GetString("UserId"),
                Name = record.GetString("Name"),
                Tag = record.GetString("Tag"),
                Timestamp = DateTime.FromBinary(record.GetLong("Timestamp"))
            };
        }

        public bool Remove(string id)
        {
            // Removes a gallery.
            var key = new Key(Namespace, setName, id);
            return DB.Delete(null, key);
        }

        public void Update(Gallery gallery)
        {
            // Creates or updates a gallery.
            var key = new Key(Namespace, setName, gallery.Id);
            Bin[] bins = {
                new Bin("UserId", gallery.UserId),
                new Bin("Name", gallery.Name),
                new Bin("Tag", gallery.Tag),
                new Bin("Timestamp", gallery.Timestamp.ToBinary())
            };
            DB.Put(writePolicy, key, bins);
        }

    }
}