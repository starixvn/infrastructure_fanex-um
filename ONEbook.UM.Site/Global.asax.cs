namespace ONEbook.UM.Site
{
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using UM.Configurations;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            UMClientManager.WhenError((message) => HttpContext.Current.Response.Write(message.Replace("\r\n", "<br/>") + "<br/>"));

            ////GlobalFilters.Filters.Add(new AuthorizeAttribute());
        }
    }
}