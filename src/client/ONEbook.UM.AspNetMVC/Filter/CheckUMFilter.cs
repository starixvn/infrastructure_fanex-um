namespace ONEbook.UM.Filters
{
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using Configurations;
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

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isUnderMaintenance = await maintenanceService.IsUnderMaintenance();

            if (isUnderMaintenance)
            {
                UnderMaintenanceAction(filterContext);
            }

            if (!string.IsNullOrWhiteSpace(configuration.ErrorUMPageUrl)
                && (await maintenanceService.GetUnderMaintenanceTime()).ConnectionResult.IsNotOk)
            {
                ErrorUnderMaintenanceAction(filterContext);
            }
        }

        public void UnderMaintenanceAction(ActionExecutingContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                response.Clear();
                response.Write("{ \"code\": 999999, \"message\": \"Under maintenance time.\" }");
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable);
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.TrySkipIisCustomErrors = true;
                response.End();
            }
            else
            {
                var underMaintenancePageAbsoluteUrl = VirtualPathUtility.ToAbsolute(configuration.UMPageUrl).ToLower();
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.Url.AbsolutePath.ToLower();
                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(configuration.UMPageUrl);
                }
            }
        }

        public void ErrorUnderMaintenanceAction(ActionExecutingContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                response.Clear();
                response.Write("{ \"code\": 999998, \"message\": \"UM Error\" }");
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable);
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.TrySkipIisCustomErrors = true;
                response.End();
            }
            else
            {
                var underMaintenancePageAbsoluteUrl = VirtualPathUtility.ToAbsolute(configuration.ErrorUMPageUrl).ToLower();
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.Url.AbsolutePath.ToLower();
                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(configuration.ErrorUMPageUrl);
                }
            }
        }
    }
}