namespace ONEbook.UM.Api
{
    using System;
    using System.Web.Http;
    using Fanex.Logging;

    public class WebApiApplication : System.Web.HttpApplication
    {
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        /// <summary>
        /// Application error event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            // Logging
            if (Server != null)
            {
                Exception ex = Server.GetLastError().GetBaseException();

                // Get base exception data
                string message = ex.Message +
                    "\r\nType: " + ex.GetType() +
                    "\r\nSource: " + ex.Source +
                    "\r\nTarget Site: " + ex.TargetSite +
                    "\r\nStack Trace: " + ex.StackTrace;

                LogService.Error(message);
            }
        }
    }
}