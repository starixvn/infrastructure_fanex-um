namespace ONEbook.UM.Transport
{
    using System;
    using Fanex.Caching;
    using Helpers;

    public class UMCacheService : IUMCacheService
    {
        private readonly ICache cacheService;

        public UMCacheService(ICache cacheService = null)
        {
            this.cacheService = cacheService ?? CacheService.CreateInstance();
        }

        public T Get<T>(string key) => cacheService.Get<T>(key);

        public void Set<T>(string key, T value, TimeSpan absoluteExpiration)
           => cacheService.Set(key, value, absoluteExpiration);
    }
}