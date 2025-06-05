using ONEbook.UM.Api.Helpers;
using ONEbook.UM.Api.Services;
using Fanex.Logging;

namespace ONEbook.UM.Api;

using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.RabbitMQ;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInternalServices(this IServiceCollection services)
    {
        services.AddSingleton<IFakeUMControlAdapter, FakeUMControlAdapter>();
        services.AddSingleton<IUMControlAdapter, UMControlAdapter>();
        services.AddSingleton<IUMTokenService, UMTokenService>();
        services.AddSingleton<IDateTimeHelper, DateTimeHelper>();
        services.AddSingleton(Logger.Log);

        return services;
    }

    public static IServiceCollection AddFanexLogger(this IServiceCollection services, IConfiguration configuration)
    {
        LogManager
            .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
            .Use(new RabbitMQLogging(configuration["Fanex.Logging:RabbitMQConnectionStrings"]));

        ExceptionHandler
            .UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());

        return services;
    }
}