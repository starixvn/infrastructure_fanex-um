namespace ONEbook.UM.Api.Models
{
    using Sunplus.UMControllerLibrary;

    public class TokenValidationRequest
    {
        public Sites SiteId { get; set; }

        public string Token { get; set; }
    }
}