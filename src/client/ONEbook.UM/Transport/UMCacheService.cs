#if NET461 || NETSTANDARD2_0

namespace ONEbook.UM.Transport;

using System;
using Fanex.Caching;
using Helpers;

public class UMCacheService(ICacheService cacheService = null) : IUMCacheService
{
    private readonly ICacheService cacheService = cacheService ?? new CacheService();

    public T Get<T>(string key) => cacheService.Get<T>(key);

    public void Remove(string key) => cacheService.Remove(key);

    public void Set<T>(string key, T value, TimeSpan absoluteExpiration)
        => cacheService.Set(key, value, new CacheItemOptions().SetAbsoluteExpiration(absoluteExpiration));
}

#endif