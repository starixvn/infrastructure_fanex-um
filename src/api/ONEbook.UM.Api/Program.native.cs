using Microsoft.OpenApi.Models;
using ONEbook.UM.Api.Configurations;
using ONEbook.UM.Api.Services;
using ONEbook.UM.Api;
using ONEbook.UM.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddInternalServices();
services.AddHttpClient<IUMControlAdapter, UMControlAdapter>();
services.Configure<FakeUMConfiguration>(configuration.GetSection(FakeUMConfiguration.FakeUM));
services.Configure<UnderMaintenanceApiConfiguration>(configuration.GetSection(UnderMaintenanceApiConfiguration.UnderMaintenanceApi));
services.AddControllers();
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ONEBookUM", Version = "v1" });
    options.CustomSchemaIds(type => type.FullName);
    var xmlFilename = $"{typeof(Program).Assembly.GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.ConfigureExceptionHandler();
    app.UseHsts();
}

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.MapGet("/", () => "ONEbook UM API!!!");

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "ONEBookUM v1"));

await app.RunAsync();
