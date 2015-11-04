namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Net;
    using System.Linq;
    using System.Web;
    using Sunplus.UMControllerLibrary;

    public class MaintenanceService : IMaintenanceService
    {
        private const string TokenName = "UMToken";
        private readonly IUMController _underMaintenanceController;

        public MaintenanceService()
            : this(null)
        {
        }

        public MaintenanceService(IUMController underMaintenanceCotnroller)
        {
            _underMaintenanceController = underMaintenanceCotnroller ?? new UMController();
        }

        public bool IsUnderMaintenance(Sites siteId)
        {
            var umTime = _underMaintenanceController.GetUnderMaintenanceTime(siteId);

            return false;
        }

        public bool IsUnderMaintenanceOnReCheckIn(Sites siteId)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetSecretTokenFromQueryString()
        {
            var request = HttpContext.Current.Request;
            if(request != null 
                && !string.IsNullOrEmpty(request.QueryString[TokenName]))
            {
                return request.QueryString[TokenName];
            }

            return string.Empty;
        }

        protected virtual bool IsRequestIPAllowToAccess()
        {
            var clientIP = GetClientIp();

            if(string.IsNullOrEmpty(clientIP))
            {
                return false;
            }

            return false;
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

    }
}