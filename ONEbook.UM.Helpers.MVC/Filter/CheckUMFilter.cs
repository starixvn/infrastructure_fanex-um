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

        public CheckUMFilterAttribute() : this(null)
        {
        }

        public CheckUMFilterAttribute(IMaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        public static void UnderMaintenanceAction(ActionExecutingContext filterContext)
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
                var underMaintenancePageAbsoluteUrl = VirtualPathUtility.ToAbsolute(UMClientConfiguration.Settings.UMPageUrl).ToLower();
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.Url.AbsolutePath.ToLower();
                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(UMClientConfiguration.Settings.UMPageUrl);
                }
            }
        }

        public static void ErrorUnderMaintenanceAction(ActionExecutingContext filterContext)
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
                var underMaintenancePageAbsoluteUrl = VirtualPathUtility.ToAbsolute(UMClientConfiguration.Settings.ErrorUMPageUrl).ToLower();
                var currentPageAbsoluteUrl = filterContext.HttpContext.Request.Url.AbsolutePath.ToLower();
                if (underMaintenancePageAbsoluteUrl != currentPageAbsoluteUrl)
                {
                    filterContext.Result = new RedirectResult(UMClientConfiguration.Settings.ErrorUMPageUrl);
                }
            }
        }

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isUnderMaintenance = await maintenanceService.IsUnderMaintenance();

            if (isUnderMaintenance)
            {
                UnderMaintenanceAction(filterContext);
            }

            if (!string.IsNullOrWhiteSpace(UMClientConfiguration.Settings.ErrorUMPageUrl)
                && (await maintenanceService.GetUnderMaintenanceTime()).ConnectionResult.IsNotOk)
            {
                ErrorUnderMaintenanceAction(filterContext);
            }
        }
    }
}