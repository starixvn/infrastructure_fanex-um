namespace ONEbook.UM.Api.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using ONEbook.UM.Api.Configurations;
    using ONEbook.UM.Api.Filters;
    using ONEbook.UM.Api.Models;
    using ONEbook.UM.Api.Services;
    using ONEbook.UM.Api.Services.Impl;

    [HmacAuthorizationFilter]
    public class TokenController : ApiController
    {
        private readonly IMaintenanceService _maintenanceService;

        public TokenController()
            : this(null)
        {
        }

        public TokenController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService ?? new MaintenanceService();
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Net.Http.HttpRequestMessageExtensions.CreateErrorResponse(System.Net.Http.HttpRequestMessage,System.Net.HttpStatusCode,System.String)", Justification = "Reviewed!"), 
        HttpGet]
        public HttpResponseMessage Validate([FromUri]TokenValidationRequest request)
        {
            if (request == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.PreconditionFailed, "Token Validation Request cannot be null!");
            }

            var isValid = _maintenanceService.IsValidUMSecretToken(request.SiteId, request.SecretToken);

            return Request.CreateResponse(HttpStatusCode.OK, isValid);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Net.Http.HttpRequestMessageExtensions.CreateErrorResponse(System.Net.Http.HttpRequestMessage,System.Net.HttpStatusCode,System.String)", Justification = "Reviewed!")]
        public HttpResponseMessage GetAll()
        {
            var siteTokens = _maintenanceService.GetAllUMSecretTokens();

            return Request.CreateResponse(HttpStatusCode.OK, siteTokens);
        }
    }
}