namespace Bps.uCloud.Backend.Consumers
{
    using Bps.Common;
    using Bps.uCloud.Contracts;
    using Bps.uCloud.Contracts.Entities;
    using MassTransit;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GalleryConsumer : IConsumer<EntityRequest<Gallery>>
    {
        private readonly IMongoDatabase db;
        private readonly IMongoCollection<Gallery> collection;

        public GalleryConsumer(IMongoDatabase DB)
        {
            db = DB;
            collection = db.GetCollection<Gallery>(nameof(Gallery));
        }

        public Task Consume(ConsumeContext<EntityRequest<Gallery>> context)
        {
            Gallery command = context.Message.Command;
            try
            {
                switch (context.Message.Task)
                {
                    case Tasks.List:
                        Exceptions.Trace("Querying galleries by user id {0}.", command.UserId);
                        return context.RespondAsync(collection.Find(x => x.UserId == command.UserId).ToList());
                    case Tasks.New:
                        Exceptions.Trace("Creating gallery {0}.", command.Id);
                        collection.InsertOne(command);
                        return Task.CompletedTask;
                    case Tasks.View:
                        Exceptions.Trace("Querying gallery by id {0}.", command.Id);
                        return context.RespondAsync(collection.Find(x => x.Id == command.Id).First() ?? Gallery.Null);
                    case Tasks.Remove:
                        Exceptions.Trace("Deleting gallery {0}.", command.Id);
                        collection.DeleteOne(x => x.Id == command.Id);
                        return Task.CompletedTask;
                    default:
                        Exceptions.Trace("Undefined gallery task {0}!", context.Message.Task);
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Exceptions.Handle(ex);
                return Task.FromException(ex);
            }
        }
    }
}