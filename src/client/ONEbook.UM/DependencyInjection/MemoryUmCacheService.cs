#if NET5_0_OR_GREATER

using Microsoft.Extensions.Caching.Memory;
using System;

namespace ONEbook.UM.DependencyInjection;

using Helpers;

public class MemoryUmCacheService(IMemoryCache memoryCache) : IUMCacheService
{
    public T Get<T>(string key) => memoryCache.Get<T>(key);

    public void Set<T>(string key, T value, TimeSpan absoluteExpiration)
        => memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration));

    public void Remove(string key) => memoryCache.Remove(key);
}

#endif