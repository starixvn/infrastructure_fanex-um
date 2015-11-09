namespace ONEbook.UM.Api.Services
{
    using System.Collections.Generic;
    using Sunplus.UMControllerLibrary;

    public interface IMaintenanceService
    {
        bool IsValidUMSecretToken(Sites siteId, string secretToken);

        string GetUMSecretToken(Sites siteId);

        IDictionary<Sites, string> GetAllUMSecretToken();
    }
}