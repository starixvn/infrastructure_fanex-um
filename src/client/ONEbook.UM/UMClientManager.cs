namespace ONEbook.UM
{
    using System;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;

    public class UMClientManager
    {
        private IUMClientConfiguration umClientConfig;
        private IRestClient restClient;
        private IHttpContextHelper httpContext;

        private static UMClientManager CurrentInstance = new UMClientManager();

        public static UMClientManager UseConfig(IUMClientConfiguration umClientConfig)
        {
            CurrentInstance.umClientConfig = umClientConfig ?? throw new ArgumentNullException(nameof(umClientConfig));

            return CurrentInstance;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public UMClientManager UseRestClient(IRestClient restClient)
        {
            CurrentInstance.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));

            return CurrentInstance;
        }

        public UMClientManager UseHttpContext(IHttpContextHelper httpContext)
        {
            CurrentInstance.httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

            return CurrentInstance;
        }

        public UMClientManager AddErrorHandler(Action<string> errorHandler)
        {
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler), "Error handler can not be null");

            return CurrentInstance;
        }

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        public static IUMClientConfiguration UMClientConfigs
          => CurrentInstance.umClientConfig ?? throw new InvalidOperationException(nameof(CurrentInstance.umClientConfig));

        public static IRestClient RestClient
            => CurrentInstance.restClient ?? throw new InvalidOperationException(nameof(CurrentInstance.restClient));

        public static IHttpContextHelper HttpContext
            => CurrentInstance.httpContext ?? throw new InvalidOperationException(nameof(CurrentInstance.httpContext));

        internal static Action<string> ErrorHandler { get; private set; }
    }
}