namespace ONEbook.UM.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using ONEbook.UM.Configurations;
    using RestSharp;

    public class RestClient : Helpers.IRestClient
    {
        private static readonly object LockObject = new object();
        private static IList<string> _errorServiceUrls = new List<string>();
        private IRestClient restClient;
        private static bool _resetErrorServiceList = false;

        public RestClient(string baseUrl = "", IRestClient restClient = null)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                this.restClient = restClient ?? CreateDefaultClient();
            }
            else
            {
                this.restClient = CreateClient(baseUrl);
            }
        }

        public static IRestClient CreateDefaultClient()
               => new RestSharp.RestClient
               {
                   BaseUrl = new Uri(UMClientConfiguration.ServiceUriList.FirstOrDefault()),
                   Authenticator = new HmacAuthenticator(),
                   Timeout = 15000
               };

        public static IRestClient CreateClient(string baseUrl)
              => new RestSharp.RestClient
              {
                  BaseUrl = new Uri(baseUrl),
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

        public void KeepTrackErrorService()
        {
            string baseUrl = restClient.BaseUrl.ToString();

            if (_errorServiceUrls.Contains(baseUrl))
            {
                return;
            }

            lock (LockObject)
            {
                if (!_errorServiceUrls.Contains(baseUrl))
                {
                    _errorServiceUrls.Add(baseUrl);
                }
            }
        }

        public void InitSurvivedRestClient()
        {
            if (!_errorServiceUrls.Any())
            {
                restClient = CreateDefaultClient();
            }

            CreateClient(GetNewServiceUrl());
        }

        private static string GetNewServiceUrl()
        {
            var newServiceUrl = UMClientConfiguration.ServiceUriList.Except(_errorServiceUrls).FirstOrDefault();

            if (newServiceUrl == null)
            {
                if (!_resetErrorServiceList)
                {
                    lock (LockObject)
                    {
                        if (_errorServiceUrls.Count > 0)
                        {
                            _errorServiceUrls = new List<string>();
                            _resetErrorServiceList = true;
                            newServiceUrl = UMClientConfiguration.ServiceUriList.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    throw new Exception("All ONEbook UM Service are dead");
                }
            }

            return newServiceUrl;
        }
    }
}