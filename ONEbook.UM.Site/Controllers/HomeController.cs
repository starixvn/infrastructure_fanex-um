namespace ONEbook.UM.Site.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using ONEbook.UM.Transport;
    using UM.Filters;

    [AllowAnonymous]
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
            _maintenanceService = maintenanceService ?? new MaintenanceService(new RestClient(), new HttpContextHelper());
        }

        public async Task<ActionResult> Login()
        {
            var isUM = await _maintenanceService.IsUnderMaintenance();

            var contentResult = new ContentResult();

            if (isUM)
            {
                var underMaintenanceInfo = await _maintenanceService.GetUnderMaintenanceTime();
                contentResult.Content = $"Site {UMClientConfiguration.Settings.SiteId} is in under maintenance "
                    + $" from {underMaintenanceInfo.From} to {underMaintenanceInfo.To} GMT+8.";
            }
            else
            {
                contentResult.Content = $"Site {UMClientConfiguration.Settings.SiteId} is not in under maintenance.";
            }

            return contentResult;
        }

        [CheckUMFilter]
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

        [HttpPost, CheckUMFilter]
        public JsonResult RecheckInWithAttributeAndPostAction()
        {
            var contentResult = new ContentResult();
            contentResult.Content = "Recheck in OK";
            return Json(contentResult);
        }

        public ActionResult ClearSession()
        {
            Session.Clear();
            Session.Abandon();

            var contentResult = new ContentResult();
            contentResult.Content = "Clear session done";
            return contentResult;
        }

        public async Task<ActionResult> RecheckIn()
        {
            var isPassUM = !(await _maintenanceService.IsUnderMaintenance());

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} {1} under maintenance on recheckin.",
                UMClientConfiguration.Settings.SiteId,
                isPassUM ? "passes" : "does not pass");

            return contentResult;
        }

        public ActionResult GetUMInfo()
        {
            return Json(
                _maintenanceService.GetUnderMaintenanceTime(),
                JsonRequestBehavior.AllowGet);
        }

        [CheckUMFilter]
        public ActionResult UMPage()
        {
            var contentResult = new ContentResult();
            contentResult.Content = "UM PAGE";
            return contentResult;
        }

        [CheckUMFilter]
        public ActionResult ErrorUMPage()
        {
            var contentResult = new ContentResult();
            contentResult.Content = "Error UM Page";
            return contentResult;
        }
    }
}