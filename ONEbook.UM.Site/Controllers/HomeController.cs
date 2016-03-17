namespace ONEbook.UM.Site.Controllers
{
    using System.Web.Mvc;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using Sunplus.UMControllerLibrary;
    using UM.Filters;
    public class HomeController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public HomeController()
            : this(null)
        {
        }

        public HomeController(
            IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        public ActionResult Login()
        {
            var isUM = _maintenanceService.IsUnderMaintenance();

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} is {1} under maintenance on login.",
                (Sites)UMClientConfiguration.Settings.SiteId,
                isUM ? string.Empty : "not");

            return contentResult;
        }

        [UMBackdoorFilter]
        public ActionResult LoginWithFilter()
        {
            var contentResult = new ContentResult();
            contentResult.Content = "Login OK";
            return contentResult;
        }

        [CheckUMFilter]
        public ActionResult RecheckInWithAttribute()
        {
            var contentResult = new ContentResult();
            contentResult.Content = "Recheck in OK";
            return contentResult;
        }

        public ActionResult ClearSession()
        {
            Session.Clear();
            Session.Abandon();

            var contentResult = new ContentResult();
            contentResult.Content = "Clear session done";
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
                _maintenanceService.GetUnderMaintenanceTime(), 
                JsonRequestBehavior.AllowGet);
        }
    }
}