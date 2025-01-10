using Microsoft.AspNetCore.Mvc;

namespace TestOnebookNet.Controllers
{
    using System.Text;
    using Newtonsoft.Json;
    using ONEbook.UM.Services;

    [ApiController]
    [Route("")]
    public class HomeController(IMaintenanceService maintenanceService) : ControllerBase
    {
        [HttpGet]
        public async Task<string> Get()
        {
            var stringBuilder = new StringBuilder();

            var time = await maintenanceService.GetUnderMaintenanceTimeAsync();
            var all = await maintenanceService.GetAllUnderMaintenanceTimeAsync();
            var boolVal = await maintenanceService.IsUnderMaintenanceAsync();

            stringBuilder.AppendLine(JsonConvert.SerializeObject(time));
            stringBuilder.AppendLine(JsonConvert.SerializeObject(all));
            stringBuilder.AppendLine(JsonConvert.SerializeObject(boolVal));
            return stringBuilder.ToString();
        }
    }
}
