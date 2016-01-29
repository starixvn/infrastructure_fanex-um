namespace ONEbook.UM.Api.Filters
{
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Fanex.Logging;
    using ONEbook.UM.Api.Helpers;

    public class LogInfoFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);

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