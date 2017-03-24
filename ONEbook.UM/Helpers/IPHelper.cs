namespace ONEbook.UM.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Web;
    using Fanex.Logging;

    /// <summary>
    /// Helper class for getting current client IP address.
    /// </summary>
    internal static class IPHelper
    {
        public static string GetClientIPAddress(HttpRequest request)
        {
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
            IPAddress internalIPAddress = GetInternalIPAddress(userHostAddress);

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

        public static string GetMachineIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        public static bool IsAuthorizedIP(string clientIP, IList<Tuple<string, string>> allowedIPs)
        {
            foreach (var allowIP in allowedIPs)
            {
                if (allowIP.Item1 == allowIP.Item2
                    && allowIP.Item1 == clientIP)
                {
                    return true;
                }

                var clientAddress = IPAddress.Parse(clientIP);
                var fromAddress = IPAddress.Parse(allowIP.Item1);
                var toAddress = IPAddress.Parse(allowIP.Item2);

                if (IsIPInRange(clientAddress, fromAddress, toAddress))
                {
                    return true;
                }
            }

            LogService.Info("User IP is not allowed: " + clientIP);

            return false;
        }

        private static bool IsIPInRange(IPAddress clientAddress, IPAddress fromAddress, IPAddress toAddress)
        {
            AddressFamily rangeAddressFamily = fromAddress.AddressFamily;
            byte[] fromAddressBytes = fromAddress.GetAddressBytes();
            byte[] toAddressBytes = toAddress.GetAddressBytes();

            if (clientAddress.AddressFamily != rangeAddressFamily)
            {
                return false;
            }

            byte[] addressBytes = clientAddress.GetAddressBytes();

            bool fromAddressBoundary = true, toAddressBoundary = true;

            for (int i = 0; i < fromAddressBytes.Length &&
                (fromAddressBoundary || toAddressBoundary); i++)
            {
                if ((fromAddressBoundary && addressBytes[i] < fromAddressBytes[i]) ||
                    (toAddressBoundary && addressBytes[i] > toAddressBytes[i]))
                {
                    return false;
                }

                fromAddressBoundary &= addressBytes[i] == fromAddressBytes[i];
                toAddressBoundary &= addressBytes[i] == toAddressBytes[i];
            }

            return true;
        }
    }
}