using ONEbook.UM.AspNetMVC;

namespace ONEbook.UM.Site
{
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Caching;
    using Helpers;
    using ONEbook.UM.AspNetMVC.Filters;
    using ONEbook.UM.AspNetMVC.SimpleInjector;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.NETFramework;
    using SimpleInjector;
    using SimpleInjector.Integration.Web.Mvc;
    using Transport;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new Container();
            //container.RegisterInstance(new UMClientConfiguration().Initialize());
            //container.RegisterInstance<ICacheService>(new CacheService());
            //container.RegisterSingleton<IUMCacheService, UMCacheService>();
            //container.RegisterSingleton<IPHelper, IPHelperImpl>();
            //container.RegisterSingleton<IUMRestClient, UMRestClient>();
            //container.RegisterSingleton<IHttpContextHelper, HttpContextHelper>();
            //container.RegisterSingleton<IMaintenanceService, MaintenanceService>();
            container.RegisterONEbookUM();
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var umConfig = new UMClientConfiguration().Initialize();

            UMClientManager
                .UseConfig(umConfig)
                .UseCacheService(new UMCacheService(new CacheService()))
                .UseRestClient(new UMRestClient(umConfig))
                .UseHttpContext(new HttpContextHelper());
        }
    }
}