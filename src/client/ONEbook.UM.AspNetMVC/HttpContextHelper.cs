namespace ONEbook.UM.Helpers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web;

    public class HttpContextHelper : IHttpContextHelper
    {
        public string GetSession(string key)
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }

            return HttpContext.Current.Session[key]?.ToString();
        }

        public void SetSession(string key, string value)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            HttpContext.Current.Session[key] = value;
        }

        public string GetValueFromQueryString(string queryKey)
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }

            return HttpContext.Current.Request.QueryString[queryKey];
        }

        public string GetIpAddress()
        {
            if (HttpContext.Current == null)
            {
                return string.Empty;
            }

            var request = HttpContext.Current.Request;

            var forwardHeader = GetForwardHeader(request);

            if (!string.IsNullOrEmpty(forwardHeader))
            {
                return forwardHeader;
            }

            return GetInternalAddress(request);
        }

        private static string GetForwardHeader(HttpRequest request)
        {
            var forwardHeader = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (forwardHeader != null)
            {
                return forwardHeader.Split(',').First().Trim();
            }

            return string.Empty;
        }

        private static string GetInternalAddress(HttpRequest request)
        {
            string userHostAddress = request.UserHostAddress;
            var internalIPAddress = GetInternalIPAddress(userHostAddress);

            if (internalIPAddress == null)
            {
                var hostName = Dns.GetHostName();

                internalIPAddress = GetInternalIPAddress(hostName);
            }

            return internalIPAddress != null ? internalIPAddress.ToString() : string.Empty;
        }

        private static IPAddress GetInternalIPAddress(string hostAddress)
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