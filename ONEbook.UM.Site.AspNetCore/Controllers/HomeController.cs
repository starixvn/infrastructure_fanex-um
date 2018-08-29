using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.Models;
using ONEbook.UM.Services;

namespace ONEbook.UM.Site.AspNetCore.Controllers
{
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
        [HttpGet]
        public async Task<ActionResult<UnderMaintenanceTime>> Index()
        {
            var underMaintenanceInfo = await maintenanceService.GetUnderMaintenanceTime();

            return underMaintenanceInfo;
        }
    }
}