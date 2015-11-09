namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly IUMController _underMaintenanceController;
        private readonly IRestClient _restClient;
        private IList<string> _allowedIPs;
        private string _clientIP;

        public MaintenanceService()
            : this(null, null, null, null)
        {
        }

        public MaintenanceService(
            IUMController underMaintenanceCotnroller,
            IRestClient restClient,
            IList<string> allowedIPs,
            string clientIP)
        {
            _underMaintenanceController = underMaintenanceCotnroller ?? new UMController();
            _restClient = restClient ?? new RestClient(RandomHelper.GetRandomUri(UMClientConfiguration.Settings.ServiceUris));
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

        public bool IsUnderMaintenance(Sites siteId)
        {
            var underMaintenanceTime = _underMaintenanceController.GetUnderMaintenanceTime(siteId);

            if (!underMaintenanceTime.IsUnderMaintenanceTime)
            {
                return false;
            }

            var secretToken = GetSecretTokenFromQueryString();

            if (string.IsNullOrEmpty(secretToken)
                || !IsRequestIPAllowToAccess())
            {
                return true;
            }

            if (!IsUMSecretTokenValid(siteId, secretToken))
            {
                return true;
            }

            ByPassUM = true;

            return false;
        }

        private bool IsUMSecretTokenValid(Sites siteId, string secretToken)
        {
            var request = new RestRequest("/undermaintenance/isvalidsecrettoken", Method.GET);
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
            var underMaintenanceTime = _underMaintenanceController.GetUnderMaintenanceTime(siteId);

            if (!underMaintenanceTime.IsUnderMaintenanceTime)
            {
                return false;
            }

            return !ByPassUM;
        }
    }
}