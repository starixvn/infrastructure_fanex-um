namespace ONEbook.UM.Filters
{
    using System.Web.Mvc;
    using Services;
    using Services.Impl;

    public sealed class UMBackdoorFilter : ActionFilterAttribute
    {
        private IMaintenanceService _maintenanceService;

        public UMBackdoorFilter() : this(null)
        {
        }

        public UMBackdoorFilter(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isUM = _maintenanceService.IsUnderMaintenance();

            if (isUM)
            {
                CheckUMFilter.UnderMaintenanceAction(filterContext);
            }
        }
    }
}