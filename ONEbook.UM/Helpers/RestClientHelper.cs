namespace ONEbook.UM.Helpers
{
    using Configurations;
    using RestSharp;

    internal static class RestClientHelper
    {
        internal static IRestClient GetRestClient()
        {
            var restClient = new RestClient(RandomHelper.GetRandomUri(UMClientConfiguration.Settings.ServiceUris));
            restClient.Authenticator = new HmacAuthenticator();

            return restClient;
        }
    }
}