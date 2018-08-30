namespace ONEbook.UM.Helpers
{
    using System.Net;
    using System.Net.Sockets;
    using ONEbook.UM.Models;

    public interface IPHelper
    {
        string GetMachineIP();
    }

    public class IPHelperImpl : IPHelper
    {
        public string GetMachineIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new UMClientException("No network adapters with an IPv4 address in the system!");
        }
    }
}