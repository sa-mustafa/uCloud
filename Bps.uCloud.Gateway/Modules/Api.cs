namespace Bps.uCloud.Gateway.Modules
{
    using Bps.uCloud.Contracts;
    using Bps.uCloud.Contracts.Entities;
    using MassTransit;
    using Microsoft.Extensions.Configuration;
    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Security;
    using Nancy.Validation;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Api : NancyModule
    {
        private readonly string userName;
        private readonly string uploadPath;

        public Api(Settings.IAppSettings settings, IBusControl bus, ISendEndpoint sendEndpoint)
        : base("/api")
        {
            //this.bus = bus;
            //this.configuration = configuration;
            //settings.ServiceUri = new Uri(configuration["RabbitMQ:services:queue"]);
            uploadPath = Path.Combine(settings.RootPath, settings.UploadDirectory);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Reference material: https://restfulapi.net/rest-put-vs-post
            // Requests must be sent with app_id & app_key.
            // Request, Context, Helper
            // For added security use OwaspHeaders.Core library.

            // http://foreverframe.net/microservices-http-forwarding-and-RestEase
            // For creates/updates/deletes(CUD) create a command and publish it to the service bus based on a queue like RabbitMQ.
            // For reads(R) forward the HTTP request to the internal API (not publicly exposed) of the particular micro-service.

            // The requests should possibly be processed without storing in queue.
            // Long-running requests should go to queue before getting processed.
            // Event correlation is a technique for making sense of a large number of events.
            // Event correlation is the requirement where you really need a bus. 
            // However, this bus is not an ESB, but an (in-memory) event server.

            //this.RequiresAuthentication();
            //userName = Context.CurrentUser.Identity.Name;
            userName = "test";

            Get("/", _ => View["index"]);

            Get("/hello", _ => Response.AsText("hello"));

            #region Gallery API

            // Get the list of all galleries (aka List() function)
            Get("/gallery", async (_) =>
            {
                var client = bus.CreateRequestClient<EntityRequest<Gallery>, IGallery[]>(settings.ServiceUri, settings.Timeout);
                var response = await client.Request(Gallery.Request(Tasks.List, userName));
                return Response.AsJson(response);
            });

            // Create a new gallery (aka New() function)
            Post("/gallery", async (_) =>
            {
                var gallery = this.Bind<Gallery>();
                gallery.UserId = userName;
                await sendEndpoint.Send(Gallery.Request(Tasks.New, gallery));
                return Response.AsText(gallery.Id);
            });

            // View a gallery by id (aka View() function)
            Get("/gallery/{id}", async (parameters) =>
            {
                var client = bus.CreateRequestClient<EntityRequest<Gallery>, IGallery>(settings.ServiceUri, settings.Timeout);
                var response = await client.Request(Gallery.Request(Tasks.View, userName, parameters.id));
                return Response.AsJson((IGallery)response);
            });

            // Delete a given gallery from DB for the current user (aka Remove() function)
            Delete("/gallery/{id}", async (parameters) =>
            {
                await sendEndpoint.Send(Gallery.Request(Tasks.Remove, userName, parameters.id));
                return Response.AsJson((string)parameters.id);
            });

            #endregion

            #region Items API

            // Get the list of items enrolled in the given gallery (aka List() function)
            Get("/gallery/{id}/items", async (parameters) =>
            {
                var client = bus.CreateRequestClient<EntityRequest<Item>, IItem[]>(settings.ServiceUri, settings.Timeout);
                var response = await client.Request(Item.Request(Tasks.List, userName, parameters.id));
                return Response.AsJson((IItem[])response);
            });

            // Create a new item in the specified gallery (aka New() function)
            Post("/gallery/{gallery}/items/{id}", async (parameters) =>
            {
                var upload = this.Bind<Data>();

                var validator = new Requests.DataValidator(settings);
                var result = validator.Validate(upload);
                if (!result.IsValid)
                    throw new InvalidDataException(result.ToString());

                EntityRequest<Item> request = Item.Request(Tasks.New, userName, parameters.gallery, parameters.id);
                // We may handle different data formats/encodings
                request.Command.Data = upload.Bytes;
                request.Command.Type = upload.Type;

                await sendEndpoint.Send(request);
                return Response.AsJson((string)parameters.gallery);
            });

            // Get an item from the given gallery (aka View() function)
            Get("/gallery/{gallery}/items/{id}", async (parameters) =>
            {
                var client = bus.CreateRequestClient<EntityRequest<Item>, IItem>(settings.ServiceUri, settings.Timeout);
                var response = await client.Request(Item.Request(Tasks.View, userName, parameters.gallery, parameters.id));
                return Response.AsJson((IItem)response);
            });

            // Delete an item from the specified gallery (aka Remove() function)
            Delete("/gallery/{gallery}/items/{id}", async (parameters) =>
            {
                await sendEndpoint.Send(Item.Request(Tasks.Remove, userName, parameters.gallery, parameters.id));
                return Response.AsJson((string)parameters.id);
            });

            #endregion

            // Perform detection on an item in the specified gallery
            Get("/gallery/{gallery}/items/{id}/detect", async (parameters) =>
            {
                EntityRequest<Item> request = Item.Request(Tasks.Process, userName, parameters.gallery, parameters.id);
                request.Command.Operation = Operations.Detect;

                await sendEndpoint.Send(request);
                return Response.AsJson((string)parameters.id);
            });

            // Perform enrollment on an item in the specified gallery
            Get("/gallery/{gallery}/items/{id}/enroll", async (_) =>
            {
            });

            // Perform identification on an item in the specified gallery
            Get("/gallery/{gallery}/items/{id}/identify", async (_) =>
            {
            });
        }

        /// <summary>
        /// Checks whether the user is authenticated.
        /// </summary>
        /// <returns>true if the user is authenticated; false otherwise.</returns>
        public bool CheckUserAuthenticity()
        {
            string appId = Request.Headers["app_id"].FirstOrDefault();
            string appKey = Request.Headers["app_key"].FirstOrDefault();
            return true;
        }

        public string HandleUpload(/*string fileName, */Stream stream)
        {
            string guid = Guid.NewGuid().ToString();
            string target = Path.Combine(uploadPath, guid);
            using (FileStream destination = File.Create(target))
            {
                stream.CopyTo(destination);
            }
            return guid; // Negotiate.WithStatusCode(HttpStatusCode.OK).WithModel()
        }

        public async Task<string> HandleUploadAsync(/*string fileName, */Stream stream)
        {
            string guid = Guid.NewGuid().ToString();
            string target = Path.Combine(uploadPath, guid);
            using (FileStream destination = File.Create(target))
            {
                await stream.CopyToAsync(destination);
            }
            return guid;

            //var file = new Response();
            //file.Headers["Content-Disposition"] = "attachment; filename=afile.pdf";
            //file.ContentType = "application/pdf";
            //file.Contents = str =>
            //{
            //    using (var writer = new System.IO.StreamWriter(str))
            //    {
            //        writer.Write( ... );
            //    };
            //};
            //return file;
        }

    }
}