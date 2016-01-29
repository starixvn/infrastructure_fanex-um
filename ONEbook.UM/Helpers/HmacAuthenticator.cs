namespace ONEbook.UM.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Fanex.Logging;
    using ONEbook.UM.Configurations;
    using RestSharp;
    using RestSharp.Authenticators;

    /// <summary>
    /// http://bitoftech.net/2014/12/15/secure-asp-net-web-api-using-api-key-authentication-hmac-authentication/
    /// </summary>
    public class HmacAuthenticator : IAuthenticator
    {
        private readonly string _machineIP;

        public HmacAuthenticator()
        {
            _machineIP = IPHelper.GetClientIPAddress(HttpContext.Current.Request);
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client", "Rest Client cannot be null");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request", "Rest Request cannot be null");
            }

            string requestUri = HttpUtility.UrlEncode(client.BuildUri(request).OriginalString.ToLower());

            LogService.Info("requestUri: " + requestUri);

            string requestHttpMethod = request.Method.ToString();

            // Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            // create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            // Creating the raw signature string
            string signatureRawData = string.Format("{0}{1}{2}{3}{4}", _machineIP, requestHttpMethod, requestUri, requestTimeStamp, nonce);

            var secretKeyByteArray = Convert.FromBase64String(UMClientConfiguration.Settings.SecretKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                // Setting the values in the Authorization header using custom scheme (amx)
                string authHeader = string.Format("amx {0}", string.Format("{0}:{1}:{2}:{3}", _machineIP, requestSignatureBase64String, nonce, requestTimeStamp));
                request.AddParameter("Authorization", authHeader, ParameterType.HttpHeader);
            }
        }
    }
}