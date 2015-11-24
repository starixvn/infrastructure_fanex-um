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
    using RestSharp;
    using Sunplus.UMControllerLibrary;

    public class MaintenanceService : IMaintenanceService
    {
        private const string TokenName = "UMToken";
        private const string ByPassUMSessionKey = "ONEbook.UM.ByPassUM";
        private readonly IUMControlAdapter _underMaintenanceControlAdapter;
        private readonly IRestClient _restClient;
        private readonly ILogger _logger;
        private IList<Tuple<string, string>> _allowedIPs;
        private string _clientIP;
        private Sites _siteId;

        public MaintenanceService()
            : this(null, null, null, null, null, 0)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed!")]
        public MaintenanceService(
            IUMControlAdapter underMaintenanceControlAdapter,
            IRestClient restClient,
            IList<Tuple<string, string>> allowedIPs,
            string clientIP,
            ILogger logger,
            Sites siteId)
        {
            _underMaintenanceControlAdapter = underMaintenanceControlAdapter ?? new UMControlAdapter();
            _restClient = restClient ?? GetRestClient();
            _allowedIPs = allowedIPs ?? UMClientConfiguration.AllowedIPs;
            _clientIP = clientIP;
            _logger = logger ?? Logger.Log;
            _siteId = (int)siteId == 0 ? (Sites)UMClientConfiguration.Settings.SiteId : siteId;
        }

        public bool ByPassUM
        {
            get
            {
                if (HttpContext.Current.Session[ByPassUMSessionKey] == null)
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

        public bool IsUnderMaintenanceOnLoginPage()
        {
            CheckSiteIdIsValid();

            if (NowIsNotUnderMaintenance())
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

        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Reviewed")]
        private void CheckSiteIdIsValid()
        {
            if (!Enum.IsDefined(typeof(Sites), _siteId))
            {
                throw new ArgumentOutOfRangeException("_siteId", "Site Id is not valid");
            }
        }

        private bool NowIsNotUnderMaintenance()
        {
            var underMaintenanceTime = _underMaintenanceControlAdapter.GetUnderMaintenanceTime(_siteId);

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
            var clientIP = _clientIP ?? IPHelper.GetClientIPAddress(HttpContext.Current.Request);

            return IPHelper.IsAuthorizedIP(clientIP, _allowedIPs);
        }

        public bool IsPassUnderMaintenance()
        {
            CheckSiteIdIsValid();

            if (ByPassUM)
            {
                return true;
            }

            return NowIsNotUnderMaintenance();
        }

        private IRestClient GetRestClient()
        {
            var restClient = new RestClient(RandomHelper.GetRandomUri(UMClientConfiguration.Settings.ServiceUris));
            restClient.Authenticator = new HmacAuthenticator();

            return restClient;
        }
    }
}