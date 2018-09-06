namespace ONEbook.UM.Site.AspNetCore.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using ONEbook.UM.Filters;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;

    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IMaintenanceService maintenanceService;

        public HomeController(IMaintenanceService maintenanceService)
        {
            this.maintenanceService = maintenanceService;
        }

        // GET api/home
        [CheckUMFilter]
        [HttpGet]
        public async Task<ActionResult<UnderMaintenanceTime>> Index()
        {
            var underMaintenanceInfo = await maintenanceService.GetUnderMaintenanceTime();

            return underMaintenanceInfo;
        }
    }
}