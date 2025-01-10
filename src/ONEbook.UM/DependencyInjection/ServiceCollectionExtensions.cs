using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;

namespace ONEbook.UM.DependencyInjection;
using Filters;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOneBookUm(this IServiceCollection services, IConfiguration configuration)
    {
        var umConfig = new UMClientConfiguration();
        configuration.Bind("ONEbook.UM", umConfig);

        services.AddMemoryCache();
        services.AddSingleton(umConfig);
        services.AddScoped<CheckUMFilterAttribute>();
        services.AddSingleton<IUMCacheService, MemoryUmCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();
        services.AddSingleton<IPHelper, IPHelperImpl>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
        services.AddSingleton<IMaintenanceService, MaintenanceService>();
        
        return services;
    }
}