
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.Api.Models;
using ONEbook.UM.Api.Services;

namespace ONEbook.UM.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UnderMaintenanceController : ControllerBase
{
    private readonly IUMControlAdapter underMaintenanceControlAdapter;

    public UnderMaintenanceController(IUMControlAdapter underMaintenanceControlAdapter)
    {
        this.underMaintenanceControlAdapter = underMaintenanceControlAdapter;
    }

    [HttpGet("GetAll")]
    public async Task<IDictionary<int, UnderMaintenanceTime>> GetAll()
        => await underMaintenanceControlAdapter.GetAllUnderMaintenanceTime();

    [HttpGet("Get")]
    public async Task<UnderMaintenanceTime> Get(int siteId)
        => await underMaintenanceControlAdapter.GetUnderMaintenanceTime(siteId);


    [HttpGet("GetScheduledUnderMaintenance")]
    public async Task<IDictionary<int, UnderMaintenanceTime>> GetScheduledUnderMaintenance()
       => await underMaintenanceControlAdapter.GetScheduledUnderMaintenanceInformation();
}
