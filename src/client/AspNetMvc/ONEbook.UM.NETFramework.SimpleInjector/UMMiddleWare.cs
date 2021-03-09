using Fanex.Caching;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.NETFramework;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using SimpleInjector;

namespace ONEbook.UM.AspNetMVC.SimpleInjector
{
    public static class ContainerExtension
    {
        public static void RegisterONEbookUM(this Container container)
        {
            container.RegisterInstance(new UMClientConfiguration().Initialize());
            container.RegisterInstance<ICacheService>(new CacheService());
            container.RegisterSingleton<IUMCacheService, UMCacheService>();
            container.RegisterSingleton<IPHelper, IPHelperImpl>();
            container.RegisterSingleton<IUMRestClient, UMRestClient>();
            container.RegisterSingleton<IMaintenanceService, MaintenanceService>();
        }
    }
}