namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using RestSharp;
    using Sunplus.UMControllerLibrary;

    public class MaintenanceService : IMaintenanceService
    {
        private const string TokenName = "UMToken";
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
            var clientIP = _clientIP ?? GetClientIp();

            return _allowedIPs.Contains(clientIP);
        }

        private string GetClientIp()
        {
            var forwardHeader = GetForwardHeader();

            if (!string.IsNullOrEmpty(forwardHeader))
            {
                return forwardHeader;
            }

            return GetInternalAddress();
        }

        private string GetForwardHeader()
        {
            var httpRequest = HttpContext.Current.Request;
            var forwardHeader = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (forwardHeader != null)
            {
                return forwardHeader.Split(',').First().Trim();
            }

            return string.Empty;
        }

        private string GetInternalAddress()
        {
            var httpRequest = HttpContext.Current.Request;
            string userHostAddress = httpRequest.UserHostAddress;
            var internalIPAddress = GetInternalIPAddress(userHostAddress);

            if (internalIPAddress == null)
            {
                var hostName = Dns.GetHostName();

                internalIPAddress = GetInternalIPAddress(hostName);
            }

            return internalIPAddress != null ? internalIPAddress.ToString() : string.Empty;
        }

        private IPAddress GetInternalIPAddress(string hostAddress)
        {
            foreach (var itemIPA in Dns.GetHostAddresses(hostAddress))
            {
                if (itemIPA.AddressFamily.ToString().Equals("InterNetwork", StringComparison.OrdinalIgnoreCase))
                {
                    return itemIPA;
                }
            }

            return null;
        }

        public bool IsUnderMaintenanceOnRecheckIn(Sites siteId)
        {
            throw new NotImplementedException();
        }
    }
}