using ONEbook.UM.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOneBookUm(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapControllers();

await app.RunAsync();
