namespace ONEbook.UM.Api.Filters
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Fanex.Logging;
    using ONEbook.UM.Api.Helpers;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class LogInfoFilterAttribute : ActionFilterAttribute
    {
        [SuppressMessage("Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Fanex.Logging.LogService.Info(System.String)",
            Justification = "Reviewed!")]
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            if (actionContext == null)
            {
                return;
            }

            var actionInfo = string.Format(
                    "Controller: {0}, Action: {1} is executing.",
                    actionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    actionContext.ActionDescriptor.ActionName);

            var message = string.Join(
                "\r\n",
                actionInfo,
                "Client IP: " + actionContext.Request.GetClientIPAddress(),
                "Url: " + actionContext.Request.RequestUri.OriginalString,
                "Body: " + actionContext.Request.GetBodyForm(),
                string.Empty);

            LogService.Info(message);
        }
    }
}