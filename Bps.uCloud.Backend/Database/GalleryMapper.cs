namespace Bps.uCloud.Backend.Database
{
    using Bps.uCloud.Contracts.Entities;
    using MongoDB.Driver;
    using System.Collections.Generic;

    public class GalleryMapper
    {
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Gallery> collection;

        public GalleryMapper(IMongoDatabase db)
        {
            this.db = db;
            collection = db.GetCollection<Gallery>(nameof(Gallery));
        }

        public bool Clear(string userId)
        {
            // Scan items by gallery id.
            return collection.DeleteMany(obj => obj.UserId == userId).DeletedCount != 0;
        }

        public List<Gallery> List(string userId)
        {
            // Inquires for galleries by user id.
            return collection.Find(obj => obj.UserId == userId).ToList();
        }

        public void New(Gallery gallery)
        {
            // Creates a new obj.
            collection.InsertOne(gallery);
        }

        public Gallery View(string id)
        {
            // Inquires for a gallery by id.
            return collection.Find(obj => obj.Id == id).First() ?? Gallery.Null;
        }

        public bool Remove(string id)
        {
            // Removes a obj.
            return collection.DeleteOne(obj => obj.Id == id).IsAcknowledged;
        }

        public void Update(Gallery gallery)
        {
            // Updates a obj.
            collection.ReplaceOne(obj => obj.Id == gallery.Id, gallery);
        }

    }
}