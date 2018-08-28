namespace ONEbook.UM.Filters
{
    using System.Net;
    using System.Threading.Tasks;
    using Configurations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Http.Extensions;
    using Services;

    public sealed class CheckUMFilterAttribute : ActionFilterAttribute
    {
        private readonly IMaintenanceService maintenanceService;

        public CheckUMFilterAttribute(IMaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var isUnderMaintenance = await maintenanceService.IsUnderMaintenance();

            if (isUnderMaintenance)
            {
                await UnderMaintenanceAction(context);
            }

            if (!string.IsNullOrWhiteSpace(UMClientConfiguration.Settings.ErrorUMPageUrl)
                && (await maintenanceService.GetUnderMaintenanceTime()).ConnectionResult.IsNotOk)
            {
                await ErrorUnderMaintenanceAction(context);
            }
        }

        public static async Task UnderMaintenanceAction(ActionExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            if (IsAjax(filterContext.HttpContext.Request))
            {
                response.Clear();
                await response.WriteAsync("{ \"code\": 999999, \"message\": \"Under maintenance time.\" }");
                filterContext.Result = new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
                await filterContext.Result.ExecuteResultAsync(filterContext);
            }
            else
            {
                var underMaintenancePageAbsoluteUrl = UriHelper.BuildAbsolute(
                    UMClientConfiguration.Settings.UMPageUrl,
                    filterContext.HttpContext.Request.Host);
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.GetDisplayUrl();

                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(UMClientConfiguration.Settings.UMPageUrl);
                }
            }
        }

        public static async Task ErrorUnderMaintenanceAction(ActionExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            if (IsAjax(filterContext.HttpContext.Request))
            {
                response.Clear();
                await response.WriteAsync("{ \"code\": 999998, \"message\": \"UM Error\" }");
                filterContext.Result = new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
                await filterContext.Result.ExecuteResultAsync(filterContext);
            }
            else
            {
                var underMaintenancePageAbsoluteUrl = UriHelper.BuildAbsolute(
                   UMClientConfiguration.Settings.ErrorUMPageUrl,
                   filterContext.HttpContext.Request.Host);
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.GetDisplayUrl();

                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(UMClientConfiguration.Settings.ErrorUMPageUrl);
                }
            }
        }

        private static bool IsAjax(HttpRequest request)
            => request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}