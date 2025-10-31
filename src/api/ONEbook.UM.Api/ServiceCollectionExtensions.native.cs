using ONEbook.UM.Api.Helpers;
using ONEbook.UM.Api.Services;

namespace ONEbook.UM.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInternalServices(this IServiceCollection services)
    {
        services.AddSingleton<IFakeUMControlAdapter, FakeUMControlAdapter>();
        services.AddSingleton<IUMControlAdapter, UMControlAdapter>();
        services.AddSingleton<IUMTokenService, UMTokenService>();
        services.AddSingleton<IDateTimeHelper, DateTimeHelper>();

        return services;
    }
}