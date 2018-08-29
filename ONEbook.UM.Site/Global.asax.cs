namespace ONEbook.UM.Site
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using ONEbook.UM.AspNetMVC.Configuration;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Transport;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var umConfig = new UMClientConfiguration();

            UMClientManager
                .UseConfig(umConfig)
                .UseRestClient(new RestClient(umConfig))
                .UseHttpContext(new HttpContextHelper());

            ////GlobalFilters.Filters.Add(new AuthorizeAttribute());
        }
    }
}