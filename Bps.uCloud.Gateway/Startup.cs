namespace Bps.uCloud.Gateway
{
    using Bps.Common;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Nancy.Owin;
    using System;
    using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;
    using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

    public class Startup : IStartup
    {
        #region Constructor

        public Startup(IConfiguration configuration)
        {
            // Save the static class instance
            Instance = this;
            Configuration = configuration;
        }

        #endregion

        #region Public Properties

        public IConfiguration Configuration { get; }

        public IBusControl Bus { get; private set; }

        public ILogger Logger { get; private set; }

        public static Startup Instance { get; set; }

        #endregion

        #region Methods

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        void IStartup.Configure(IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetService<IHostingEnvironment>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseMvc();
            app.UseOwin(pipeline => pipeline.UseNancy(options =>
            {
                options.Bootstrapper = new Bootstrapper(Configuration);
            }));

            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            lifetime.ApplicationStarted.Register(OnStarted);
            lifetime.ApplicationStopping.Register(OnStopped);

            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            Logger = app.ApplicationServices.GetService<ILogger<GatewayService>>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        IServiceProvider IStartup.ConfigureServices(IServiceCollection services)
        {
            // Add MVC & other framework services
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddSingleton(Configuration);

            // Configure the MassTransit service bus.
            Bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                // If you need to only send/publish messages, don’t create any receive endpoints.
                // The bus automatically creates a temporary queue.
                cfg.Host(
                    new Uri(Configuration["RabbitMQ:uri"]),
                    busConfig =>
                    {
                        busConfig.Username(Configuration["RabbitMQ:user"]);
                        busConfig.Password(Configuration["RabbitMQ:pswd"]);
                    });
                //ConnectConsumer/CircuitBreaker/RateLimiter/GetProbeResult/PrefetchCount
            });

            return services.BuildServiceProvider();
        }

        public void OnStarted()
        {
            try
            {
                Instance.Logger.LogTrace("{0} is running...", Program.AppName);
                Instance.Bus.Start();
            }
            catch (Exception ex)
            {
                Exceptions.Handle(ex);
            }
        }

        public void OnStopped()
        {
            try
            {
                Instance.Bus?.Stop(TimeSpan.FromSeconds(30));
                Instance.Logger.LogTrace("Stopped {0}. Good bye!", Program.AppName);
                //Exceptions.Shutdown();
            }
            catch (Exception ex)
            {
                Exceptions.Handle(ex);
            }
        }

        #endregion
    }
}