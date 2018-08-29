namespace ONEbook.UM.Helpers
{
    using System.Net;
    using System.Net.Sockets;

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

            return "127.0.0.1";
        }
    }
}