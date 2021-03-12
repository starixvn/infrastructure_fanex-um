using ONEbook.UM.AspNetMVC.Filters;

namespace ONEbook.UM.Site.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Services;

    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IMaintenanceService _maintenanceService;

        public HomeController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService
                  ?? new MaintenanceService(
                      UMClientManager.RestClient,
                      UMClientManager.CacheService,
                      UMClientManager.UMClientConfigs,
                      UMClientManager.HttpContext);
        }

        [CheckUMFilter]
        public async Task<ActionResult> Login()
        {
            var isUM = await _maintenanceService.IsUnderMaintenanceAsync();

            var contentResult = new ContentResult();

            if (isUM)
            {
                var underMaintenanceInfo = await _maintenanceService.GetUnderMaintenanceTimeAsync();
                contentResult.Content = $"Site is in under maintenance "
                    + $" from {underMaintenanceInfo.From} to {underMaintenanceInfo.To} GMT+8.";
            }
            else
            {
                contentResult.Content = $"Site is not in under maintenance.";
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
            var isPassUM = !(await _maintenanceService.IsUnderMaintenanceAsync());

            var contentResult = new ContentResult();
            contentResult.Content = string.Format(
                "Site {0} {1} under maintenance on recheckin.",
                UMClientManager.UMClientConfigs.SiteId,
                isPassUM ? "passes" : "does not pass");

            return contentResult;
        }

        public ActionResult GetUMInfo()
        {
            return Json(
                _maintenanceService.GetUnderMaintenanceTimeAsync(),
                JsonRequestBehavior.AllowGet);
        }

        public string GetScheduledUMInfo()
        {
            return JsonConvert.SerializeObject(_maintenanceService.GetScheduledUnderMaintenanceAsync().GetAwaiter().GetResult());
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