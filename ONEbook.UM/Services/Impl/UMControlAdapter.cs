namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using Fanex.Logging;
    using Helpers;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using RestSharp;

    public class UMControlAdapter : IUMControlAdapter
    {
        private const string SiteUMCacheKey = "ONEbook.UM.Site.{0}";
        private const string FormatTime = "HH:mm tt";
        private readonly int _siteId;
        private readonly ICachingService _cachingService;
        private readonly IRestClient _restClient;
        private readonly ILogger _logger;

        public UMControlAdapter()
            : this(UMClientConfiguration.Settings.SiteId, new CachingService(), null, Logger.Log)
        {
        }

        internal UMControlAdapter(
            int siteId,
            ICachingService cachingService,
            IRestClient restClient,
            ILogger logger)
        {
            _siteId = siteId;
            _cachingService = cachingService;
            _restClient = restClient ?? RestClientHelper.GetRestClient();
            _logger = logger;
        }

        public UnderMaintenanceTime GetUnderMaintenanceTime()
        {
            if (UMClientConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UMClientConfiguration.Settings.UMFrom, UMClientConfiguration.Settings.UMTo);
            }

            if (UMClientConfiguration.Settings.EnableCheckUM)
            {
                return _cachingService.GetOrAdd(
                    string.Format(SiteUMCacheKey, _siteId),
                    () =>
                    {
                        return GetUnderMaintenance();
                    },
                    DateTime.Now.AddSeconds(UMClientConfiguration.Settings.ResultCacheDuration));
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed!")]
        private UnderMaintenanceTime GetUnderMaintenance()
        {
            var request = new RestRequest("/UnderMaintenance/get", Method.GET);
            request.AddObject(new { siteId = _siteId });
            request.Timeout = 15000;

            try
            {
                IRestResponse response = _restClient.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return SimpleJson.DeserializeObject<UnderMaintenanceTime>(response.Content);
                }
                else
                {
                    _logger.Warn(string.Format("{0} \r\n {1}", response.Content, response.ErrorMessage ?? string.Empty));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }
    }
}