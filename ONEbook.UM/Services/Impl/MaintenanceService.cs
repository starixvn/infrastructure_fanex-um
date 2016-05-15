namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Web;
    using Fanex.Logging;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Models;
    using RestSharp;

    public class MaintenanceService : IMaintenanceService
    {
        private const string TokenName = "UMToken";
        private const string ByPassUMSessionKey = "ONEbook.UM.ByPassUM";
        private readonly IUMControlAdapter _underMaintenanceControlAdapter;
        private readonly IRestClient _restClient;
        private readonly ILogger _logger;
        private readonly ICachingService _cachingService;
        private IList<Tuple<string, string>> _allowedIPs;
        private string _clientIP;
        private int _siteId;
        private bool _isAllowAllIPs;

        public MaintenanceService()
            : this(null, null, null, null, null, null, 0, null)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed!")]
        internal MaintenanceService(
            IUMControlAdapter underMaintenanceControlAdapter,
            IRestClient restClient,
            IList<Tuple<string, string>> allowedIPs,
            string clientIP,
            ILogger logger,
            ICachingService cachingService,
            int siteId,
            bool? isAllowAllIPs)
        {
            _logger = logger ?? Logger.Log;
            _restClient = restClient ?? GetRestClient();
            _siteId = (int)siteId == 0 ? UMClientConfiguration.Settings.SiteId : siteId;
            _cachingService = cachingService ?? new CachingService();
            _underMaintenanceControlAdapter = underMaintenanceControlAdapter ?? new UMControlAdapter(_siteId, _cachingService, _restClient, _logger);
            _allowedIPs = allowedIPs ?? UMClientConfiguration.AllowedIPs;
            _clientIP = clientIP;
            _isAllowAllIPs = isAllowAllIPs.HasValue ? isAllowAllIPs.Value : UMClientConfiguration.Settings.IPList.AllowAll;
        }

        public bool ByPassUM
        {
            get
            {
                if (HttpContext.Current == null 
                    || HttpContext.Current.Session[ByPassUMSessionKey] == null)
                {
                    return false;
                }

                return (bool)HttpContext.Current.Session[ByPassUMSessionKey];
            }

            private set
            {
                HttpContext.Current.Session[ByPassUMSessionKey] = value;
            }
        }

        public bool IsUnderMaintenance()
        {
            if (NowIsNotUnderMaintenance()
                || ByPassUM)
            {
                return false;
            }

            var token = GetTokenFromQueryString();

            if (!string.IsNullOrEmpty(token)
                && IsRequestIPAllowToAccess()
                && IsTokenValid(token))
            {
                ByPassUM = true;

                return false;
            }

            return true;
        }

        private bool NowIsNotUnderMaintenance()
        {
            var underMaintenanceTime = _underMaintenanceControlAdapter.GetUnderMaintenanceTime();

            return !underMaintenanceTime.IsUnderMaintenanceTime;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed!")]
        private bool IsTokenValid(string token)
        {
            var request = new RestRequest("/token/validate", Method.GET);
            request.AddObject(new { siteId = _siteId, token = token });

            try
            {
                IRestResponse response = _restClient.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Convert.ToBoolean(response.Content);
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

            return false;
        }

        private string GetTokenFromQueryString()
        {
            var request = HttpContext.Current.Request;
            if (request != null
                && !string.IsNullOrEmpty(request.QueryString[TokenName]))
            {
                return request.QueryString[TokenName];
            }

            return string.Empty;
        }

        private bool IsRequestIPAllowToAccess()
        {
            if (_isAllowAllIPs)
            {
                return true;
            }

            var clientIP = _clientIP ?? IPHelper.GetClientIPAddress(HttpContext.Current.Request);

            return IPHelper.IsAuthorizedIP(clientIP, _allowedIPs);
        }

        private IRestClient GetRestClient()
        {
            var restClient = new RestClient(RandomHelper.GetRandomUri(UMClientConfiguration.Settings.ServiceUris));
            restClient.Authenticator = new HmacAuthenticator();

            return restClient;
        }

        public UnderMaintenanceTime GetUnderMaintenanceTime()
        {
            return _underMaintenanceControlAdapter.GetUnderMaintenanceTime();
        }
    }
}