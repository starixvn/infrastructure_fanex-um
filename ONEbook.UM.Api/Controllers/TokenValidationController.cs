namespace ONEbook.UM.Api.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using ONEbook.UM.Api.Services;
    using ONEbook.UM.Api.Services.Impl;
    using Sunplus.UMControllerLibrary;

    public class TokenValidationController : ApiController
    {
        private readonly IMaintenanceService _maintenanceService;

        public TokenValidationController()
            : this(null)
        {
        }

        public TokenValidationController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        public HttpResponseMessage Get(Sites siteId, string secretToken)
        {
            var isValid = _maintenanceService.IsValidUMSecretToken(siteId, secretToken);

            return Request.CreateResponse(HttpStatusCode.OK, isValid);
        }
    }
}