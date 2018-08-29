namespace ONEbook.UM.Site.AspNetCore
{
    using Fanex.Logging;
    using Fanex.Logging.RabbitMQ;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ONEbook.UM.AspNetCore.Configuration;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            LogManager
                 .SetDefaultLogCategory(Configuration["Fanex.Logging:DefaultCategory"])
                 .Use(new RabbitMQLogging(Configuration["Fanex.Logging:RabbitMQConnectionStrings"]));

            var umConfig = new UMClientConfiguration(Configuration);

            UMClientManager
                .UseConfig(umConfig)
                .UseRestClient(new RestClient(umConfig))
                .UseHttpContext(new HttpContextHelper(app.ApplicationServices.GetService<IHttpContextAccessor>()));
        }
    }
}