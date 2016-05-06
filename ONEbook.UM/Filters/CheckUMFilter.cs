namespace ONEbook.UM.Filters
{
    using System.Web;
    using System.Web.Mvc;
    using Configurations;
    using Services;
    using Services.Impl;

    public sealed class CheckUMFilter : ActionFilterAttribute
    {
        private IMaintenanceService _maintenanceService;

        public CheckUMFilter() : this(null)
        {
        }

        public CheckUMFilter(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        public static void UnderMaintenanceAction(ActionExecutingContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                response.Clear();
                response.Write("{ \"code\": 999999, \"message\": \"Under maintenance time.\" }");
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.TrySkipIisCustomErrors = true;
                response.End();
            }
            else
            {
                filterContext.Result = new RedirectResult(UMClientConfiguration.Settings.UMPageUrl);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isPassUM = _maintenanceService.IsPassUnderMaintenance();

            if (!isPassUM)
            {
                UnderMaintenanceAction(filterContext);
            }
        }
    }
}