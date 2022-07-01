using ONEbook.UM.Api.Helpers;
using ONEbook.UM.Api.Services;
using ONEbook.UM.Api.Extensions;
using Fanex.Logging;
using Microsoft.OpenApi.Models;
using ONEbook.UM.Api.Configurations;

namespace ONEbook.UM.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FakeUMConfiguration>(Configuration.GetSection(FakeUMConfiguration.FakeUM));
            services.Configure<UnderMaintenanceApiConfiguration>(Configuration.GetSection(UnderMaintenanceApiConfiguration.UnderMaintenanceApi));

            services.AddControllers();
            services.AddSingleton<IUMControlAdapter, UMControlAdapter>();
            services.AddSingleton<IUMTokenService, UMTokenService>();
            services.AddSingleton<IDateTimeHelper, DateTimeHelper>();
            services.AddSingleton(Logger.Log);
            services.AddHttpClient<IUMControlAdapter, UMControlAdapter>();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ONEBookUM", Version = "v1" });
                options.CustomSchemaIds(type => type.FullName);
                var xmlFilename = $"{typeof(Startup).Assembly.GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.ConfigureExceptionHandler();
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", () => "ONEbook UM API!!!");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "ONEBookUM v1"));
        }
    }
}
