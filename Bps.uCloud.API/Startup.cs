namespace Bps.uCloud.API
{
    using Bps.uCloud.API.Filters;
    using Bps.uCloud.API.Security;
    using Bps.uCloud.API.Settings;
    using GreenPipes;
    using MassTransit;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;
    using MongoDB.Driver;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.IO;
    using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

    /// <summary>
    /// Implements ASP .net core IStartup interface
    /// </summary>
    /// <seealso cref="IStartup" />
    public class Startup : IStartup
    {
        #region Fields

        private IWebHostEnvironment hostingEnv;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The environment object.</param>
        /// <param name="configuration">The configuration object.</param>
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            // Save the static class instance
            Instance = this;
            hostingEnv = env;
            Configuration = configuration;
        }

        #endregion

        #region Public Properties

        //public IBusControl Bus { get; private set; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Gets the singleton instance of <see cref="Startup"/> object.
        /// </summary>
        public static Startup Instance { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// This method gets called by the runtime. 
        /// Use this method to configure the app's request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        void IStartup.Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetService<IHostingEnvironment>();
            //app.UseHttpsRedirection();
            //app.UseMvc();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //TODO: Either use the SwaggerGen generated Swagger contract (generated from C# classes)
                c.SwaggerEndpoint("/swagger/1.0.2/swagger.json", "uCloud API collection");
                //TODO: Or alternatively use the original Swagger contract that's included in the static files
                // c.SwaggerEndpoint("/swagger-original.json", "uCloud API collection Original");
            });
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO: Enable production exception handling (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
                app.UseExceptionHandler("/Error");
            }

            app.UseHsts();

            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            //lifetime.ApplicationStarted.Register(OnStarted);
            //lifetime.ApplicationStopping.Register(OnStopped);

            using (var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()))
            {
                Logger = loggerFactory.CreateLogger<ApiService>();
            }

            Logger = app.ApplicationServices.GetService<ILogger<ApiService>>();
        }

        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddControllers(opts =>
                {
                    opts.InputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter>();
                    opts.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter>();
                    //opts.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                })
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() };
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                })
                .AddXmlSerializerFormatters();

            services
                .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, null);

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("1.0.2", new OpenApiInfo
                    {
                        Version = "1.0.2",
                        Title = "uCloud API collection",
                        Description = "uCloud API collection (ASP.NET Core 3.1)",
                        Contact = new OpenApiContact()
                        {
                            Name = "Swagger Codegen Contributors",
                            Url = new Uri("https://github.com/swagger-api/swagger-codegen"),
                            Email = "help@bps-eng.co.ir"
                        },
                        TermsOfService = new Uri("http://www.opensource.org/licenses/MIT")
                    });
                    c.CustomSchemaIds(type => type.FullName);
                    c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{hostingEnv.ApplicationName}.xml");
                    // Sets the basePath property in the Swagger document generated
                    c.DocumentFilter<BasePathFilter>("/ucloud/v1");

                    // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });

            // Configure the MassTransit service bus.
            services
                .AddMassTransit(x =>
                {
                    // If you need to only send/publish messages, don’t create any receive endpoints.
                    // The bus automatically creates a temporary queue.
                    x.UsingRabbitMq
                        ((ctx, cfg) =>
                            {
                                cfg.ConfigureEndpoints(ctx);
                                cfg.Host(
                                    new Uri(Configuration["RabbitMQ:uri"]),
                                    busConfig =>
                                    {
                                        busConfig.Username(Configuration["RabbitMQ:user"]);
                                        busConfig.Password(Configuration["RabbitMQ:pswd"]);
                                    });

                                //var queueUri = new UriBuilder(Configuration["RabbitMQ:uri"])
                                //{
                                //    Path = Configuration["RabbitMQ:queues:process"]
                                //}.Uri;

                                //cfg.ReceiveEndpoint(Configuration["RabbitMQ:queues:api"], e =>
                                //{
                                //    e.UseRetry(policy => policy.Immediate(5));
                                //    // Use a singleton thread-safe consumer for messages
                                //    //e.Instance(new Consumers.GalleryConsumer(db));
                                //    //e.Instance(new Consumers.ItemConsumer(db, Configuration["Aerospike:namespace"], queueUri));
                                //    //e.Consumer OR e.Handler
                                //});
                                //cfg.UseBsonSerializer()
                            }
                        );

                    x.SetKebabCaseEndpointNameFormatter();
                    //ConnectConsumer/CircuitBreaker/RateLimiter/GetProbeResult/PrefetchCount
                });

            services.AddMassTransitHostedService();

            ConfigureIoC(services);

            return services.BuildServiceProvider();
        }

        void ConfigureIoC(IServiceCollection services)
        {
            var client = new MongoClient(Configuration["MongoDB:uri"]);
            var db = client.GetDatabase(Configuration["MongoDB:name"]);
            services.AddSingleton(db);

            services.AddSingleton<IAppSettings>(new AppSettings(Configuration));
            services.AddSingleton(Configuration);
            //services.AddScoped<IIdentityProvider, IdentityProvider>();
            services.AddSingleton(new KeyProvider(Configuration));
        }

        //public void OnStarted()
        //{
        //    try
        //    {
        //        Instance.Logger.LogTrace("{0} is running...", Program.AppName);
        //        //Instance.Bus.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        Exceptions.Handle(ex);
        //    }
        //}

        //public void OnStopped()
        //{
        //    try
        //    {
        //        //Instance.Bus?.Stop(TimeSpan.FromSeconds(30));
        //        Instance.Logger.LogTrace("Stopped {0}. Good bye!", Program.AppName);
        //        //Exceptions.Shutdown();
        //    }
        //    catch (Exception ex)
        //    {
        //        Exceptions.Handle(ex);
        //    }
        //}

        #endregion
    }
}