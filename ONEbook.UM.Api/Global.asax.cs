namespace ONEbook.UM.Api
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http;
    using Fanex.Logging;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        /// <summary>
        /// Application error event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", 
            MessageId = "Fanex.Logging.LogService.Error(System.String)", Justification = "Reviewed!")]
        protected void Application_Error(object sender, EventArgs e)
        {
            // Logging
            if (Server != null)
            {
                Exception ex = Server.GetLastError().GetBaseException();

                // Get base exception data
                string message = ex.Message +
                    "\r\nType: " + ex.GetType().ToString() +
                    "\r\nSource: " + ex.Source +
                    "\r\nTarget Site: " + ex.TargetSite +
                    "\r\nStack Trace: " + ex.StackTrace;

                LogService.Error(message);
            }
        }
    }
}