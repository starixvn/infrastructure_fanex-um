namespace ONEbook.UM.ConsoleApp
{
    using System;
    using Fanex.Caching;
    using Configurations;
    using Helpers;
    using NETFramework;
    using Services;
    using Transport;

    /*using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;*/

    internal class Program
    {
        private static void Main()
        {
            var umConfig = new UMClientConfiguration().Initialize();
            UMClientManager
                .UseConfig(umConfig)
                .UseCacheService(new UMCacheService(new CacheService()))
                .UseRestClient(new UMRestClient(umConfig))
                .UseHttpContext(new HttpContextHelper());

            var maintenanceService = new MaintenanceService(
                      UMClientManager.RestClient,
                      UMClientManager.CacheService,
                      UMClientManager.UMClientConfigs,
                      UMClientManager.HttpContext);

            var uminfo = maintenanceService.GetUnderMaintenanceTime();
            var isUM = maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

            Console.WriteLine(isUM);

            Console.WriteLine(
                string.Join(
                "\r\n",
                "------------",
                uminfo.IsUnderMaintenanceTime,
                uminfo.From,
                uminfo.To,
                uminfo.ConnectionResult.IsOk,
                uminfo.ConnectionResult.Message));
        }

        /*private static void Test()
        {
            const string Splash = "/";
            var sb = new StringBuilder();
            sb.Append("Start");
            try
            {
                var httpClient = new HttpClient();
                var serviceUri = "http://localhost:51974/api";
                httpClient.BaseAddress = new Uri(serviceUri + Splash);
                httpClient.Timeout = TimeSpan.FromMinutes(10);
                var siteId = "2000000";
                var endPoint = $"UnderMaintenance/get?siteId={siteId}";
                var method = HttpMethod.Get;
                httpClient.DefaultRequestHeaders.Authorization = BuildAuthorizationHeader(new Uri(httpClient.BaseAddress + endPoint), method);
                using (var request = new HttpRequestMessage(method, new Uri(endPoint, UriKind.Relative)))
                {
                    var response = httpClient.SendAsync(request).Result;
                    sb.Append(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception ex)
            {
                sb.Append(ex.Message);
            }
        }

        private static AuthenticationHeaderValue BuildAuthorizationHeader(Uri url, HttpMethod method)
        {
            var requestUri = HttpUtility.UrlEncode(url.OriginalString.ToLowerInvariant());
            var requestHttpMethod = method.ToString();
            var secretKey = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=";
            var authHeader = GenerateAuthorizationHeader(secretKey, requestUri, requestHttpMethod, GetMachineIP());

            return new AuthenticationHeaderValue("amx", authHeader);
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

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GenerateAuthorizationHeader(string secretKey, string requestUri, string requestMethod, string machineIP)
        {
            // Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            // create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            // Creating the raw signature string
            string signatureRawData = string.Format("{0}{1}{2}{3}{4}", machineIP, requestMethod, requestUri, requestTimeStamp, nonce);

            var secretKeyByteArray = Convert.FromBase64String(secretKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                // Setting the values in the Authorization header using custom scheme (amx)
                string authHeader = string.Format("{0}", string.Format("{0}:{1}:{2}:{3}", machineIP, requestSignatureBase64String, nonce, requestTimeStamp));

                return authHeader;
            }
        }*/
    }
}