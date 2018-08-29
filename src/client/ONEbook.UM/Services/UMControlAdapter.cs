namespace ONEbook.UM.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Helpers;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;

    public interface IUMControlAdapter
    {
        Task<UnderMaintenanceTime> GetUnderMaintenanceTime();
    }

    public class UMControlAdapter : IUMControlAdapter
    {
        private const string SiteUMCacheKey = "ONEbook.UM.Site.{0}";
        private readonly ICacheService cacheService;
        private readonly IRestClient restClient;
        private readonly IUMClientConfiguration configuration;

        internal UMControlAdapter(
            ICacheService cacheService,
            IRestClient restClient,
            IUMClientConfiguration configuration)
        {
            this.cacheService = cacheService;
            this.restClient = restClient;
            this.configuration = configuration;
        }

        public async Task<UnderMaintenanceTime> GetUnderMaintenanceTime()
        {
            if (configuration.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, configuration.UMFrom, configuration.UMTo);
            }

            if (configuration.EnableCheckUM)
            {
                var cacheKey = string.Format(SiteUMCacheKey, configuration.SiteId);
                var umInfo = cacheService.Get<UnderMaintenanceTime>(cacheKey);

                if (umInfo == null)
                {
                    umInfo = await GetUnderMaintenance();

                    cacheService.Set(cacheKey, umInfo,
                        new CacheItemOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(configuration.ResultCacheDuration)));
                }

                return umInfo;
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }

        internal async Task<UnderMaintenanceTime> GetUnderMaintenance()
        {
            try
            {
                return await restClient.ExecuteGetAsync<UnderMaintenanceTime>(
                    $"/NewUnderMaintenance/get?siteId={configuration.SiteId}");
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