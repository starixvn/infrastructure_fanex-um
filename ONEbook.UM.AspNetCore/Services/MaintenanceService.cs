namespace ONEbook.UM.AspNetCore.Services
{
    using System;
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

                var umInfo = _cacheService.Get<UnderMaintenanceTime>(string.Format(SiteUMCacheKey, siteId));

                if (umInfo == null)
                {
                    umInfo = await SendRequestToGetUnderMaintenanceTime(siteId);

                    if (umInfo.IsUnderMaintenanceTime)
                    {
                        await _cacheService.SetAsync(
                           string.Format(SiteUMCacheKey, siteId), umInfo,
                           new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(0, 1, 0)));
                    }
                }

                return umInfo;
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