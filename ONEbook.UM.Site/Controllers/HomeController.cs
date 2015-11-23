namespace ONEbook.UM.Site.Controllers
{
    using System.Web.Mvc;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using Sunplus.UMControllerLibrary;

    public class HomeController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public HomeController()
            : this(null)
        {
        }

        public HomeController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        public ActionResult Login()
        {
            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin();

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} is {1} under maintenance on login.",
                UMClientConfiguration.Settings.SiteId, 
                isUM ? string.Empty : "not");

            return contentResult;
        }

        public ActionResult RecheckIn()
        {
            var isUM = _maintenanceService.IsUnderMaintenanceOnRecheckIn();

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} is {1} under maintenance on recheckin.",
                UMClientConfiguration.Settings.SiteId,
                isUM ? string.Empty : "not");

            return contentResult;
        }
    }
}