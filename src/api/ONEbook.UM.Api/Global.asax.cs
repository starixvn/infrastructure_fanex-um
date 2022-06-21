namespace ONEbook.UM.Api
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web.Http;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Web;
    using Fanex.Logging.RabbitMQ;
    using ONEbook.UM.Api.Helpers;
    using ONEbook.UM.Api.Services;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;

    public class WebApiApplication : System.Web.HttpApplication
    {
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        protected void Application_Start()
        {
            var appSettings = ConfigurationManager.AppSettings;
            LogManager
                .SetDefaultLogCategory(appSettings["Fanex.Logging:DefaultCategory"])
                .Use(new RabbitMQLogging(appSettings["Fanex.Logging:RabbitMQConnectionStrings"]));

            GlobalConfiguration.Configure(WebApiConfig.Register);

            var container = new Container();
            container.Register<IUMControlAdapter, UMControlAdapter>(Lifestyle.Singleton);
            container.Register<IUMTokenService, UMTokenService>(Lifestyle.Singleton);
            container.Register<IDateTimeHelper, DateTimeHelper>(Lifestyle.Singleton);
            container.RegisterInstance(Logger.Log);
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            ExceptionHandler.UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());

            Logger.Log.Info($"{Assembly.GetExecutingAssembly().GetName().Name} started");
        }

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        /// <summary>
        /// Application error event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandler.Handle(Server.GetLastError());
        }
    }
}