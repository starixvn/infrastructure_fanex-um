namespace ONEbook.UM.AspNetCore.Services
{
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using ONEbook.UM.Core.Configurations;
    using ONEbook.UM.Core.Helpers;
    using ONEbook.UM.Core.Models;
    using RestSharp;

    public interface IMaintenanceService
    {
        Task<UnderMaintenanceTime> GetUnderMaintenanceTime();
    }

    public class MaintenanceService : IMaintenanceService
    {
        private const string SiteUMCacheKey = "ONEbook.UM.Site.{0}";
        private readonly IRestClient _restClient;
        private readonly ICacheService _cacheService;

        public MaintenanceService()
            : this(null, null)
        {
        }

        public MaintenanceService(IRestClient restClient, ICacheService cacheService)
        {
            _restClient = restClient ?? RestClientHelper.GetRestClient();
            _cacheService = cacheService ?? new CacheService();
        }

        public async Task<UnderMaintenanceTime> GetUnderMaintenanceTime()
        {
            if (UMClientConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UMClientConfiguration.Settings.UMFrom, UMClientConfiguration.Settings.UMTo);
            }

            if (UMClientConfiguration.Settings.EnableCheckUM)
            {
                var siteId = UMClientConfiguration.Settings.SiteId;

                return await _cacheService.GetOrSet(string.Format(SiteUMCacheKey, siteId), async () =>
                {
                    return await SendRequestToGetUnderMaintenanceTime(siteId);
                });
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }

        private async Task<UnderMaintenanceTime> SendRequestToGetUnderMaintenanceTime(int siteId)
        {
            var request = new RestRequest("/NewUnderMaintenance/get", Method.GET);
            request.AddObject(new { siteId });

            var response = await _restClient.ExecuteTaskAsync<UnderMaintenanceTime>(request);

            if (response.StatusCode == HttpStatusCode.OK || response.Data.ErrorCode != 2)
            {
                return response.Data;
            }

            return UnderMaintenanceTime.CreateFailedConnection();
        }
    }
}