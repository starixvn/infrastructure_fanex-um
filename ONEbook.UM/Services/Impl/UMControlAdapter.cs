namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Helpers;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;

    public class UMControlAdapter : IUMControlAdapter
    {
        private const string SiteUMCacheKey = "ONEbook.UM.Site.{0}";
        private readonly ICacheService cacheService;
        private readonly IRestClient restClient;

        internal UMControlAdapter(
            ICacheService cacheService,
            IRestClient restClient)
        {
            this.cacheService = cacheService;
            this.restClient = restClient;
        }

        public async Task<UnderMaintenanceTime> GetUnderMaintenanceTime()
        {
            if (UMClientConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UMClientConfiguration.Settings.UMFrom, UMClientConfiguration.Settings.UMTo);
            }

            if (UMClientConfiguration.Settings.EnableCheckUM)
            {
                var cacheKey = string.Format(SiteUMCacheKey, UMClientConfiguration.Settings.SiteId);
                var umInfo = cacheService.Get<UnderMaintenanceTime>(cacheKey);

                if (umInfo == null)
                {
                    umInfo = await GetUnderMaintenance();

                    cacheService.Set(cacheKey, umInfo,
                        new CacheItemOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(UMClientConfiguration.Settings.ResultCacheDuration)));
                }

                return umInfo;
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }

        internal async Task<UnderMaintenanceTime> GetUnderMaintenance()
        {
            var underMaintenance = await GetUnderMaintenanceWithSurvivedService();

            if (underMaintenance.ConnectionResult != null
                && underMaintenance.ConnectionResult.IsNotOk
                && UMClientManager.ErrorHandler != null)
            {
                UMClientManager.ErrorHandler(underMaintenance.ConnectionResult.Message);
            }

            return underMaintenance;
        }

        internal async Task<UnderMaintenanceTime> GetUnderMaintenanceWithSurvivedService()
        {
            try
            {
                var umInfo = await restClient.ExecuteGetAsync<UnderMaintenanceTime>($"/NewUnderMaintenance/get?siteId={UMClientConfiguration.Settings.SiteId}");

                if (UMClientConfiguration.HasMoreThanOneService && umInfo.ConnectionResult.IsNotOk)
                {
                    restClient.KeepTrackErrorService();
                    restClient.InitSurvivedRestClient();
                    return await GetUnderMaintenanceWithSurvivedService();
                }

                return umInfo;
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException)
                {
                    return UnderMaintenanceTime.CreateFailedConnection(ex.Message);
                }

                return UnderMaintenanceTime.CreateFailedConnection(ex.Message);
            }
        }
    }
}