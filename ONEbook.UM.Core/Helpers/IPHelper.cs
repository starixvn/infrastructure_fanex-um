namespace ONEbook.UM.Core.Helpers
{
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Helper class for getting current client IP address.
    /// </summary>
    public static class IPHelper
    {
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
    }
}