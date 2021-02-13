namespace Bps.uCloud.Gateway
{
    using Bps.Common;
    using Bps.uCloud.Gateway.Security;
    using Bps.uCloud.Gateway.Settings;
    using Microsoft.Extensions.Configuration;
    using Nancy;
    using Nancy.Authentication.Stateless;
    using Nancy.Bootstrapper;
    using Nancy.Responses.Negotiation;
    using Nancy.TinyIoc;
    using System;
    using System.Diagnostics.Contracts;

    // The bootstrapper enables you to reconfigure the composition of the framework,
    // by overriding the various methods and properties.
    // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

    // AutofacNancyBootstrapper: ILifetimeScope
    // DefaultNancyBootstrapper: TinyIoCContainer

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        #region Fields

        private readonly IConfiguration configuration;
        private readonly Uri queueUri;

        #endregion

        #region Constructor

        public Bootstrapper(IConfiguration configuration)
        {
            this.configuration = configuration;
            queueUri = new UriBuilder(configuration["RabbitMQ:uri"])
                { Path = configuration["RabbitMQ:services:queue"] }.Uri;
        }

        #endregion

        #region Methods

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            Nancy.Security.Csrf.Enable(pipelines);
            //Nancy.Session.CookieBasedSessions.Enable(pipelines)
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // Perform registration that should have an application lifetime
            base.ConfigureApplicationContainer(container);

            container.Register<IAppSettings>
                (new AppSettings
                    (FileSize.Create(10, FileSize.Unit.MegaByte), 
                        RootPathProvider.GetRootPath(),
                        queueUri.AbsoluteUri, 
                        int.Parse(configuration["RabbitMQ:services:timeout"]),
                        "Uploads"));
            container.Register(Startup.Instance.Bus);

            using (var endpointTask = Startup.Instance.Bus.GetSendEndpoint(queueUri))
            {
                endpointTask.Wait();
                container.Register(endpointTask.Result);
            }
            container.Register(configuration);
            container.Register<IIdentityProvider, IdentityProvider>();
            container.Register<IKeyProvider>(new KeyProvider(configuration));
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            pipelines.OnError += (ctx, exception) => HandleException(ctx, exception, container.Resolve<IResponseNegotiator>());
            if (context.Request.Url.Path.StartsWith("/api", StringComparison.Ordinal))
            {
                //var provider = container.Resolve<IIdentityProvider>().Create();
                //StatelessAuthentication.Enable(pipelines, provider);
            }
        }

        private static Response HandleException(NancyContext context, Exception exception, IResponseNegotiator responseNegotiator)
        {
            Contract.Requires(responseNegotiator != null);
            Contract.Requires(context != null);
            Exceptions.Handle(exception);
            //return CreateNegotiatedResponse(context, responseNegotiator, exception, defaultError)
            return null;
        }

        #endregion
    }
}