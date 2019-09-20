using ONEbook.UM.AspNetMVC;

namespace ONEbook.UM.Site
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Caching;
    using Helpers;
    using ONEbook.UM.Configurations;
    using Transport;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
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