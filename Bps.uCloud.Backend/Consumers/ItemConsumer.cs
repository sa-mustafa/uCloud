namespace Bps.uCloud.Backend.Consumers
{
    using Aerospike.Client;
    using Bps.Common;
    using Bps.uCloud.Backend.Database;
    using Bps.uCloud.Contracts;
    using Bps.uCloud.Contracts.Entities;
    using MassTransit;
    using System;
    using System.Threading.Tasks;

    public class ItemConsumer : IConsumer<EntityRequest<Contracts.Entities.Item>>
    {
        private readonly DBManager dbManager;
        private readonly Lazy<ISendEndpoint> sendEndpoint;

        public ItemConsumer(IAerospikeClient DB, string Namespace, Uri sendQueue)
        {
            dbManager = new DBManager(DB, Namespace);
            sendEndpoint = new Lazy<ISendEndpoint>(() =>
            {
                using var endpoint = BackendService.Instance.Bus.GetSendEndpoint(sendQueue);
                endpoint.Wait();
                return endpoint.Result;
            });
        }

        public Task Consume(ConsumeContext<EntityRequest<Contracts.Entities.Item>> context)
        {
            Contracts.Entities.Item command = context.Message.Command;
            try
            {
                switch (context.Message.Task)
                {
                    case Tasks.List:
                        Exceptions.Trace("Querying items in gallery {0}.", command.GalleryId);
                        return context.RespondAsync(dbManager.Items.List(command.GalleryId));
                    case Tasks.New:
                        Exceptions.Trace("Creating item {0}.", command.Id);
                        dbManager.Items.New(command);
                        return Task.CompletedTask;
                    case Tasks.Process:
                        Exceptions.Trace("Processing item {0}.", command.Id);
                        return sendEndpoint.Value.Send(new DbImage(dbManager.Items.View(command.Id)) { Operation = command.Operation });
                    case Tasks.View:
                        Exceptions.Trace("Querying item by id {0}.", command.Id);
                        return context.RespondAsync(dbManager.Items.View(command.Id));
                    case Tasks.Remove:
                        Exceptions.Trace("Deleting item {0}.", command.Id);
                        dbManager.Items.Remove(command.Id);
                        return Task.CompletedTask;
                    default:
                        Exceptions.Trace("Undefined item task {0}!", context.Message.Task);
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