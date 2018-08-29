namespace ONEbook.UM.Filters
{
    using System.Net;
    using System.Threading.Tasks;
    using Configurations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Services;

    public sealed class CheckUMFilterAttribute : ActionFilterAttribute
    {
        private readonly IMaintenanceService maintenanceService;
        private readonly IUMClientConfiguration configuration;

        public CheckUMFilterAttribute() : this(null, null)
        {
        }

        public CheckUMFilterAttribute(IMaintenanceService maintenanceService, IUMClientConfiguration configuration)
        {
            this.maintenanceService = maintenanceService ?? new MaintenanceService();
            this.configuration = configuration ?? UMClientManager.UMClientConfigs;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var isUnderMaintenance = await maintenanceService.IsUnderMaintenance();

            if (isUnderMaintenance)
            {
                await UnderMaintenanceAction(context);
            }

            if (!string.IsNullOrWhiteSpace(configuration.ErrorUMPageUrl)
                && (await maintenanceService.GetUnderMaintenanceTime()).ConnectionResult.IsNotOk)
            {
                await ErrorUnderMaintenanceAction(context);
            }
        }

        public async Task UnderMaintenanceAction(ActionExecutingContext filterContext)
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
                    configuration.UMPageUrl,
                    filterContext.HttpContext.Request.Host);
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.GetDisplayUrl();

                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(configuration.UMPageUrl);
                }
            }
        }

        public async Task ErrorUnderMaintenanceAction(ActionExecutingContext filterContext)
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
                   configuration.ErrorUMPageUrl,
                   filterContext.HttpContext.Request.Host);
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.GetDisplayUrl();

                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(configuration.ErrorUMPageUrl);
                }
            }
        }

        private static bool IsAjax(HttpRequest request)
            => request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}