using ONEbook.UM.Api.Services;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.Api.Constants;

namespace ONEbook.UM.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IUMTokenService underMaintenanceTokenService;

    public TokenController(IUMTokenService underMaintenanceTokenService)
    {
        this.underMaintenanceTokenService = underMaintenanceTokenService;
    }

    [HttpGet("validate")]
    public IActionResult Validate(int siteId, string token)
    {

        var isValid = underMaintenanceTokenService.IsValidUMToken((Sites)siteId, token);

        return Ok(isValid);
    }

    [HttpGet("GetAll")]
    public IActionResult GetAll()
    {
        var siteTokens = underMaintenanceTokenService.GetAllUMTokens();

        return Ok(siteTokens);
    }
}
