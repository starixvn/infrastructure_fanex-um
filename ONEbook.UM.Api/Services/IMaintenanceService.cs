namespace ONEbook.UM.Api.Services
{
    using System.Collections.Generic;
    using ONEbook.UM.Api.Models;
    using Sunplus.UMControllerLibrary;

    public interface IMaintenanceService
    {
        bool IsValidUMSecretToken(Sites siteId, string secretToken);

        string GetUMSecretToken(Sites siteId);

        IDictionary<Sites, SiteToken> GetAllUMSecretTokens();
    }
}