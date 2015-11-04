namespace ONEbook.UM.Api.Services
{
    using Sunplus.UMControllerLibrary;

    public interface IMaintenanceService
    {
        bool IsValidUMSecretToken(Sites siteId, string secretToken);

        string GetUMSecretToken(Sites siteId);
    }
}