namespace ONEbook.UM.Api
{
    using System;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using Filters;
    using Newtonsoft.Json.Serialization;
    using ONEbook.UM.Api.Helpers;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            if (config == null)
            {
                throw new ArgumentNullException("config", "Configuration cannot be null.");
            }

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApiByAction",
                routeTemplate: "api/{controller}/{action}");

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            config.Filters.Add(new GlobalExceptionFilter());

            var formaters = GlobalConfiguration.Configuration.Formatters;
            formaters.Clear();
            formaters.Add(new JsonMediaTypeFormatter());

            var jsonFormater = formaters.JsonFormatter;
            jsonFormater.Indent = true;
            ////jsonFormater.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            ////config.Filters.Add(new LogInfoFilter());
        }
    }
}