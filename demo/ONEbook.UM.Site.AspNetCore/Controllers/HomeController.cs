namespace ONEbook.UM.Site.AspNetCore.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using ONEbook.UM.AspNetCore.Filters;
    using Services;

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
        //[ServiceFilter(typeof(CheckUMFilterAttribute))]
        [HttpGet]
        public async Task<ActionResult<UnderMaintenanceTime>> Index()
        {
            var underMaintenanceInfo = await maintenanceService.GetUnderMaintenanceTimeAsync();

            return underMaintenanceInfo;
        }
    }
}