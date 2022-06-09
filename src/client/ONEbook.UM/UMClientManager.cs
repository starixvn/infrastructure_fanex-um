namespace ONEbook.UM
{
    using System;
    using Configurations;
    using Helpers;

    public class UMClientManager
    {
        private UMClientConfiguration clientConfigs;
        private IUMRestClient restClient;
        private IUMCacheService cacheService;
        private IHttpContextHelper httpContext;
        private Action<string> errorHandler;

        private static readonly UMClientManager CurrentInstance = new UMClientManager();

        public static UMClientManager UseConfig(UMClientConfiguration clientConfigs)
        {
            CurrentInstance.clientConfigs = clientConfigs ?? throw new ArgumentNullException(nameof(clientConfigs));

            return CurrentInstance;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public UMClientManager UseRestClient(IUMRestClient restClient)
        {
            CurrentInstance.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));

            return CurrentInstance;
        }

        public UMClientManager UseHttpContext(IHttpContextHelper httpContext)
        {
            CurrentInstance.httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

            return CurrentInstance;
        }

        public UMClientManager UseCacheService(IUMCacheService cacheService)
        {
            CurrentInstance.cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

            return CurrentInstance;
        }

        public UMClientManager AddErrorHandler(Action<string> errorHandler)
        {
            CurrentInstance.errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler), "Error handler can not be null");

            return CurrentInstance;
        }

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static

        public static UMClientConfiguration UMClientConfigs
          => CurrentInstance.clientConfigs ?? throw new InvalidOperationException(nameof(CurrentInstance.clientConfigs));

        public static IUMRestClient RestClient
            => CurrentInstance.restClient ?? throw new InvalidOperationException(nameof(CurrentInstance.restClient));

        public static IUMCacheService CacheService
            => CurrentInstance.cacheService ?? throw new InvalidOperationException(nameof(CurrentInstance.cacheService));

        public static IHttpContextHelper HttpContext => CurrentInstance.httpContext;

        public static Action<string> ErrorHandler => CurrentInstance.errorHandler;
    }
}