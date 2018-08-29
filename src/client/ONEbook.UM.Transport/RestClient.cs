namespace ONEbook.UM.Transport
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using ONEbook.UM.Configurations;
    using RestSharp;

    public class RestClient : Helpers.IRestClient
    {
        private readonly IRestClient restClient;

        public RestClient(IUMClientConfiguration configuration, IRestClient restClient = null)
        {
            this.restClient = restClient ?? CreateDefaultClient(configuration);
        }

        public static IRestClient CreateDefaultClient(IUMClientConfiguration configuration)
               => new RestSharp.RestClient
               {
                   BaseUrl = new Uri(configuration.ServiceUri),
                   Authenticator = new HmacAuthenticator(),
                   Timeout = 15000
               };

        public Task<T> ExecuteGetAsync<T>(string endPoint, object data = null)
             => ExecuteSync<T>(endPoint, Method.GET, data);

        public Task<T> ExecutePostAsync<T>(string endPoint, object data = null)
            => ExecuteSync<T>(endPoint, Method.POST, data);

        private async Task<T> ExecuteSync<T>(string endPoint, Method method, object data)
        {
            ValidateEndPoint(endPoint);

            var request = new RestRequest(endPoint, method);

            if (data != null)
            {
                request.AddJsonBody(data);
            }

            var response = await restClient.ExecuteTaskAsync<T>(request).ConfigureAwait(false);

            if (response.IsSuccessful)
            {
                return response.Data;
            }

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                await Task.FromException(new Exception(response.Content));
            }

            throw new InvalidOperationException(
                string.Join(
                    "\r\n",
                    $"Content: {response.Content}",
                    $"ErrorMessage: {response.ErrorMessage}"));
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }
    }
}