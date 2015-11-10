namespace ONEbook.UM.Site.Controllers
{
    using System.Web.Mvc;
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

        // GET: Token
        public ActionResult Login(int siteId = 0)
        {
            Sites siteIdNeedBeChecked = siteId == 0 ? Sites.AccountSite : (Sites)siteId;
            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin(siteIdNeedBeChecked);

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} is {1} under maintenance on login.",
                siteIdNeedBeChecked, 
                isUM ? string.Empty : "not");

            return contentResult;
        }

        public ActionResult RecheckIn(int siteId = 0)
        {
            Sites siteIdNeedBeChecked = siteId == 0 ? Sites.AccountSite : (Sites)siteId;
            var isUM = _maintenanceService.IsUnderMaintenanceOnRecheckIn(siteIdNeedBeChecked);

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} is {1} under maintenance on recheckin.",
                siteIdNeedBeChecked,
                isUM ? string.Empty : "not");

            return contentResult;
        }
    }
}