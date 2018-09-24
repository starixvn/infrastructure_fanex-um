namespace ONEbook.UM.Api.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using ONEbook.UM.Api.Filters;
    using ONEbook.UM.Api.Models;
    using ONEbook.UM.Api.Services;

    [HmacAuthorizationFilter]
    public class TokenController : ApiController
    {
        private readonly IUMTokenService underMaintenanceTokenService;

        public TokenController()
            : this(null)
        {
        }

        public TokenController(IUMTokenService underMaintenanceTokenService)
        {
            this.underMaintenanceTokenService = underMaintenanceTokenService ?? new UMTokenService();
        }

        [HttpGet]
        public HttpResponseMessage Validate([FromUri]TokenValidationRequest request)
        {
            if (request == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Token Validation Request cannot be null!");
            }

            var isValid = underMaintenanceTokenService.IsValidUMToken(request.SiteId, request.Token);

            return Request.CreateResponse(HttpStatusCode.OK, isValid);
        }

        public HttpResponseMessage GetAll()
        {
            var siteTokens = underMaintenanceTokenService.GetAllUMTokens();

            return Request.CreateResponse(HttpStatusCode.OK, siteTokens);
        }
    }
}