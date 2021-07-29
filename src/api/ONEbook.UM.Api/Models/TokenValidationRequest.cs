using ONEbook.UM.Api.Constants;

namespace ONEbook.UM.Api.Models
{
    public class TokenValidationRequest
    {
        public Sites SiteId { get; set; }

        public string Token { get; set; }
    }
}