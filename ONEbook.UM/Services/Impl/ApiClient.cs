namespace ONEbook.UM.Services.Impl
{
    using System.Diagnostics.CodeAnalysis;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using RestSharp;

    public static class ApiClient
    {
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Reviewed!")]
        public static readonly IRestClient Client;

        private static readonly object RestClientCreatingLockObject = new object();

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Reviewed!")]
        static ApiClient()
        {
            if (Client == null)
            {
                lock (RestClientCreatingLockObject)
                {
                    if (Client == null)
                    {
                        Client = new RestClient(RandomHelper.GetRandomUri(UMClientConfiguration.Settings.ServiceUris));
                        ////Client.Authenticator = new HmacAuthenticator();
                    }
                }
            }
        }
    }
}