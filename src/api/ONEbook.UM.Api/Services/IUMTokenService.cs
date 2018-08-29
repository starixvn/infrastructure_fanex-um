namespace ONEbook.UM.Api.Services
{
    using System.Collections.Generic;
    using ONEbook.UM.Api.Models;
    using Sunplus.UMControllerLibrary;

    public interface IUMTokenService
    {
        bool IsValidUMToken(Sites siteId, string token);

        string GetUMToken(Sites siteId);

        IDictionary<Sites, SiteToken> GetAllUMTokens();
    }
}