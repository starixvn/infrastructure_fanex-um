namespace ONEbook.UM
{
    using System;
    using Configurations;
    using Helpers;

    public class UMClientManager
    {
        private UMClientConfiguration umClientConfig;
        private IUMRestClient iumRestClient;
        private IUMCacheService iumCacheService;
        private IHttpContextHelper httpContext;
        private Action<string> errorHandler;

        private static readonly UMClientManager CurrentInstance = new UMClientManager();

        public static UMClientManager UseConfig(UMClientConfiguration umClientConfig)
        {
            CurrentInstance.umClientConfig = umClientConfig ?? throw new ArgumentNullException(nameof(umClientConfig));

            return CurrentInstance;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public UMClientManager UseRestClient(IUMRestClient restClient)
        {
            CurrentInstance.iumRestClient = restClient ?? throw new ArgumentNullException(nameof(restClient));

            return CurrentInstance;
        }

        public UMClientManager UseHttpContext(IHttpContextHelper httpContext)
        {
            CurrentInstance.httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

            return CurrentInstance;
        }

        public UMClientManager UseCacheService(IUMCacheService cacheService)
        {
            CurrentInstance.iumCacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

            return CurrentInstance;
        }

        public UMClientManager AddErrorHandler(Action<string> errorHandler)
        {
            CurrentInstance.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler), "Error handler can not be null");

            return CurrentInstance;
        }

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        public static UMClientConfiguration UMClientConfigs
          => CurrentInstance.umClientConfig ?? throw new InvalidOperationException(nameof(CurrentInstance.umClientConfig));

        public static IUMRestClient RestClient
            => CurrentInstance.iumRestClient ?? throw new InvalidOperationException(nameof(CurrentInstance.iumRestClient));

        public static IUMCacheService CacheService
            => CurrentInstance.iumCacheService ?? throw new InvalidOperationException(nameof(CurrentInstance.iumCacheService));

        public static IHttpContextHelper HttpContext
            => CurrentInstance.httpContext ?? throw new InvalidOperationException(nameof(CurrentInstance.httpContext));

        public static Action<string> ErrorHandler => CurrentInstance.errorHandler;
    }
}