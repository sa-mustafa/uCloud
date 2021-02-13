namespace Bps.uCloud.Backend
{
    using Bps.Common;
    using GreenPipes;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackendService : IHostedService //BackgroundService
    {

        #region Fields

        // MongoDB NoSQL database
        private IMongoDatabase db;
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly Settings.MongoDB mongo;
        private readonly Settings.RabbitMQ rabbit;

        #endregion

        #region Properties

        public IBusControl Bus { get; private set; }

        public static BackendService Instance { get; set; }

        #endregion

        #region Methods

        public BackendService(IConfiguration configuration, ILogger<BackendService> logger, IOptions<Settings.MongoDB> mongo, IOptions<Settings.RabbitMQ> rabbit)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.mongo = mongo.Value;
            this.rabbit = rabbit.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Save the static class instance
                Instance = this;

                // Connect to the Aerospike DB
                db = new MongoClient(mongo.uri).GetDatabase(mongo.name);

                // Configure the MassTransit service bus.
                Bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    // Commands are sent (not published), whereas messages are published.
                    cfg.Host
                        (
                            rabbit.uri,
                            busConfig =>
                            {
                                busConfig.Username(rabbit.user);
                                busConfig.Password(rabbit.pswd);
                            }
                        );

                    var queueUri = new UriBuilder(rabbit.uri)
                        { 
                            Path = configuration["RabbitMQ:queues:process"] 
                        }.Uri;

                    cfg.ReceiveEndpoint(configuration["RabbitMQ:queues:api"], e =>
                    {
                        e.UseRetry(policy => policy.Immediate(5));
                        // Use a singleton thread-safe consumer for messages
                        e.Instance(new Consumers.GalleryConsumer(db));
                        e.Instance(new Consumers.ItemConsumer(db, configuration["Aerospike:namespace"], queueUri));
                        //e.Consumer OR e.Handler
                    });
                    //cfg.UseBsonSerializer()
                });

                Bus.Start();
                Console.WriteLine("{0} is running..." , Program.AppName);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Exceptions.Handle(ex);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Bus?.Stop(TimeSpan.FromSeconds(30));
            db?.Dispose();
            Console.WriteLine("Stopped {0}. Good bye!", Program.AppName);
            //Exceptions.Shutdown()
            return Task.CompletedTask;
        }
     
        #endregion
    }
}