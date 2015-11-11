namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Web;
    using Fanex.UM.Helpers;
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
        private IList<string> _allowedIPs;
        private string _clientIP;

        public MaintenanceService()
            : this(null, null, null, null)
        {
        }

        public MaintenanceService(
            IUMControlAdapter underMaintenanceControlAdapter,
            IRestClient restClient,
            IList<string> allowedIPs,
            string clientIP)
        {
            _underMaintenanceControlAdapter = underMaintenanceControlAdapter ?? new UMControlAdapter();
            _restClient = restClient ?? GetRestClient();
            _allowedIPs = allowedIPs ?? UMClientConfiguration.AllowedIPs;
            _clientIP = clientIP;
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

        public bool IsUnderMaintenanceOnLogin(Sites siteId)
        {
            CheckSiteIdIsValid(siteId);

            if (NowIsNotUnderMaintenance(siteId))
            {
                return false;
            }

            var secretToken = GetSecretTokenFromQueryString();

            if (!string.IsNullOrEmpty(secretToken)
                && IsRequestIPAllowToAccess()
                && IsSecretTokenValid(siteId, secretToken))
            {
                ByPassUM = true;

                return false;
            }

            return true;
        }

        private void CheckSiteIdIsValid(Sites siteId)
        {
            if (!Enum.IsDefined(typeof(Sites), siteId))
            {
                throw new ArgumentOutOfRangeException("siteId", "Site Id is not valid");
            }
        }

        private bool NowIsNotUnderMaintenance(Sites siteId)
        {
            var underMaintenanceTime = _underMaintenanceControlAdapter.GetUnderMaintenanceTime(siteId);

            return !underMaintenanceTime.IsUnderMaintenanceTime;
        }

        private bool IsSecretTokenValid(Sites siteId, string secretToken)
        {
            var request = new RestRequest("/token/validate", Method.GET);
            request.AddObject(new { siteId = siteId, secretToken = secretToken });

            IRestResponse response = _restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Convert.ToBoolean(response.Content);
            }

            return false;
        }

        private string GetSecretTokenFromQueryString()
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

            return _allowedIPs.Contains(clientIP);
        }

        public bool IsUnderMaintenanceOnRecheckIn(Sites siteId)
        {
            CheckSiteIdIsValid(siteId);

            if (NowIsNotUnderMaintenance(siteId))
            {
                return false;
            }

            return !ByPassUM;
        }

        private IRestClient GetRestClient()
        {
            var restClient = new RestClient(RandomHelper.GetRandomUri(UMClientConfiguration.Settings.ServiceUris));
            restClient.Authenticator = new HmacAuthenticator();

            return restClient;
        }
    }
}