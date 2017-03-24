namespace ONEbook.UM.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurations;
    using RestSharp;

    internal static class RestClientHelper
    {
        private static readonly object LockObject = new object();
        private static string _currentServiceUrl;
        private static IList<string> _errorServiceUrls = new List<string>();
        private static bool _resetErrorServiceList = false;

        internal static IRestClient GetRestClient()
        {
            var currentServiceUrl = _currentServiceUrl ?? UMClientConfiguration.ServiceUriList.FirstOrDefault();

            return GetRestClient(currentServiceUrl);
        }

        internal static IRestClient GetRestClient(string url)
        {
            var restClient = new RestClient(new Uri(url));
            restClient.Authenticator = new HmacAuthenticator();
            return restClient;
        }

        internal static IRestClient GetSurvivedRestClient()
        {
            if (!_errorServiceUrls.Any())
            {
                return GetRestClient();
            }

            _currentServiceUrl = GetNewServiceUrl();

            return GetRestClient(_currentServiceUrl);
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
                        if (_errorServiceUrls.Any())
                        {
                            //// Try reset error services once
                            _errorServiceUrls = new List<string>();
                            _resetErrorServiceList = true;
                            newServiceUrl = UMClientConfiguration.ServiceUriList.FirstOrDefault();
                        }
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("All ONEbook UM Service are dead");
                }
            }

            return newServiceUrl;
        }

        internal static void KeepTrackErrorService(string errorServiceUrl)
        {
            if (_errorServiceUrls.Contains(errorServiceUrl))
            {
                return;
            }

            lock (LockObject)
            {
                if (!_errorServiceUrls.Contains(errorServiceUrl))
                {
                    _errorServiceUrls.Add(errorServiceUrl);
                }
            }
        }
    }
}