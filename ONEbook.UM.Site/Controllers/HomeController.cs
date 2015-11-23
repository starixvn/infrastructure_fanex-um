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
        private readonly IUMControlAdapter _underMaintenanceControl;

        public HomeController()
            : this(null, null)
        {
        }

        public HomeController(
            IMaintenanceService maintenanceService,
            IUMControlAdapter underMaintenanceControl)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
            _underMaintenanceControl = underMaintenanceControl ?? new UMControlAdapter();
        }

        public ActionResult Login()
        {
            var isUM = _maintenanceService.IsUnderMaintenanceOnLoginPage();

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} is {1} under maintenance on login.",
                (Sites)UMClientConfiguration.Settings.SiteId,
                isUM ? string.Empty : "not");

            return contentResult;
        }

        public ActionResult RecheckIn()
        {
            var isPassUM = _maintenanceService.IsPassUnderMaintenance();

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} {1} under maintenance on recheckin.",
                (Sites)UMClientConfiguration.Settings.SiteId,
                isPassUM ? "passes" : "does not pass");

            return contentResult;
        }

        public ActionResult GetUMInfo()
        {
            return Json(
                _underMaintenanceControl.GetUnderMaintenanceTime((Sites)UMClientConfiguration.Settings.SiteId), 
                JsonRequestBehavior.AllowGet);
        }
    }
}