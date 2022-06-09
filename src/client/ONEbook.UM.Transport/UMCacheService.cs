namespace ONEbook.UM.Transport
{
    using System;
    using Fanex.Caching;
    using Helpers;

    public class UMCacheService : IUMCacheService
    {
        private readonly ICacheService cacheService;

        public UMCacheService(ICacheService cacheService = null)
        {
            this.cacheService = cacheService ?? new CacheService();
        }

        public T Get<T>(string key) => cacheService.Get<T>(key);

        public void Set<T>(string key, T value, TimeSpan absoluteExpiration)
           => cacheService.Set(key, value, new CacheItemOptions().SetAbsoluteExpiration(absoluteExpiration));
    }
}