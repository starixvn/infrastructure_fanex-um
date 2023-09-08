using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.RabbitMQ;

namespace ONEbook.UM.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        InitFanexLogger(configuration);
        var appName = typeof(Program).Assembly.GetName().Name;

        try
        {
            Logger.Log.Info($"Starting {appName}");
            CreateHostBuilder(configuration, args).Build().Run();
            Logger.Log.Info($"{appName} has been started up.");
        }
        catch (Exception ex)
        {
            Logger.Log.Error($"Program terminated unexpectedly {appName}", ex);
        }
    }

    private static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://0.0.0.0:5000");
            });

    private static void InitFanexLogger(IConfiguration configuration)
    {
        LogManager
            .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
            .Use(new RabbitMQLogging(configuration["Fanex.Logging:RabbitMQConnectionStrings"]));

        ExceptionHandler
            .UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());
    }

    private static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}

