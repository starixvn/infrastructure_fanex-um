namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using Helpers;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using RestSharp;

    public class UMControlAdapter : IUMControlAdapter
    {
        private const string SiteUMCacheKey = "ONEbook.UM.Site.{0}";
        private const int RestClientTimeout = 15000;
        private readonly int _siteId;
        private readonly ICachingService _cachingService;
        private readonly IRestClient _restClient;
        private readonly ILoggingService _loggingService;

        public UMControlAdapter()
            : this(UMClientConfiguration.Settings.SiteId, new CachingService(), null, new LoggingService())
        {
        }

        internal UMControlAdapter(
            int siteId,
            ICachingService cachingService,
            IRestClient restClient,
            ILoggingService loggingService)
        {
            _siteId = siteId;
            _cachingService = cachingService;
            _restClient = restClient;
            _loggingService = loggingService;
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
        internal UnderMaintenanceTime GetUnderMaintenance()
        {
            var request = new RestRequest("/UnderMaintenance/get", Method.GET);
            request.AddObject(new { siteId = _siteId });
            request.Timeout = RestClientTimeout;

            var restClient = _restClient ?? RestClientHelper.GetRestClient();

            var underMaintenance = GetUnderMaintenanceWithSurvivedService(restClient, request);

            if (underMaintenance.ConnectionResult != null
                && underMaintenance.ConnectionResult.IsNotOk
                && UMClientManager.ErrorHandler != null)
            {
                UMClientManager.ErrorHandler(underMaintenance.ConnectionResult.Message);
            }

            return underMaintenance;
        }

        internal UnderMaintenanceTime GetUnderMaintenance(IRestClient restClient, RestRequest request)
        {
            try
            {
                IRestResponse response = restClient.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return SimpleJson.DeserializeObject<UnderMaintenanceTime>(response.Content);
                }

                var logMessage = LogErrorMessage(response, restClient.BaseUrl);

                return UnderMaintenanceTime.CreateFailedConnection(logMessage);
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex.Message, ex);

                return UnderMaintenanceTime.CreateFailedConnection(ex.Message);
            }
        }

        internal UnderMaintenanceTime GetUnderMaintenanceWithSurvivedService(IRestClient restClient, RestRequest request)
        {
            try
            {
                var underMaintenance = GetUnderMaintenance(restClient, request);

                if (UMClientConfiguration.HasMoreThanOneService
                    && underMaintenance.ConnectionResult.IsNotOk)
                {
                    RestClientHelper.KeepTrackErrorService(restClient.BaseUrl.ToString());

                    return GetUnderMaintenanceWithSurvivedService(RestClientHelper.GetSurvivedRestClient(), request);
                }

                return underMaintenance;
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

        private string LogErrorMessage(IRestResponse restResponse, Uri baseRequestUri)
        {
            var logMessage = string.Join(
                    "\r\n",
                    "Base Url: " + baseRequestUri,
                    "Status Code: " + (int)restResponse.StatusCode,
                    "Response Content: " + restResponse.Content,
                    "Response Error Message: " + (restResponse.ErrorMessage ?? string.Empty));

            _loggingService.Error(logMessage);

            return logMessage;
        }
    }
}