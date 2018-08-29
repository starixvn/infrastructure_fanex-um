namespace ONEbook.UM.Api.Helpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Web.Http.Filters;
    using Fanex.Logging;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "System.Net.Http.HttpRequestMessageExtensions.CreateErrorResponse(System.Net.Http.HttpRequestMessage,System.Net.HttpStatusCode,System.String)",
            Justification = "Reviewed")]
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null)
            {
                var exception = actionExecutedContext.Exception;
                var messageBuilder = new StringBuilder();

                messageBuilder.AppendFormat("Message: {0}", exception.Message).AppendLine();
                messageBuilder.AppendFormat("Type: {0}", exception.GetType().ToString()).AppendLine();
                messageBuilder.AppendFormat("Source: {0}", exception.Source).AppendLine();
                messageBuilder.AppendFormat("Client IP: {0}", actionExecutedContext.Request.GetClientIPAddress()).AppendLine();
                messageBuilder.AppendFormat("Culture: {0}", Thread.CurrentThread.CurrentUICulture).AppendLine();
                messageBuilder.AppendFormat("Url: {0}", actionExecutedContext.Request.RequestUri.OriginalString).AppendLine();
                messageBuilder.AppendFormat("Body: {0}", actionExecutedContext.Request.GetBodyForm()).AppendLine();

                LogService.Error(messageBuilder.ToString(), actionExecutedContext.Exception);

                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
    }
}