namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Runtime.Caching;

    /// <summary>
    ///  <see cref="https://github.com/alastairtree/LazyCache"/>
    /// </summary>
    internal class CachingService : ICachingService
    {
        private readonly ObjectCache _cache;

        public CachingService()
            : this(MemoryCache.Default)
        {
        }

        public CachingService(ObjectCache cache)
        {
            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }

            this._cache = cache;
            DefaultCacheDuration = 60 * 20;
        }

        /// <summary>
        ///     Seconds to cache objects for by default
        /// </summary>
        public int DefaultCacheDuration { get; set; }

        private DateTimeOffset DefaultExpiryDateTime
        {
            get
            {
                return DateTimeOffset.Now.AddSeconds(DefaultCacheDuration);
            }
        }

        public void Add<T>(string key, T item)
        {
            Add(key, item, DefaultExpiryDateTime);
        }

        public void Add<T>(string key, T item, DateTimeOffset absoluteExpiration)
        {
            Add(key, item, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }

        public void Add<T>(string key, T item, TimeSpan slidingExpiration)
        {
            Add(key, item, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }

        public void Add<T>(string key, T item, CacheItemPolicy policy)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            ValidateKey(key);

            _cache.Set(key, item, policy);
        }

        public T Get<T>(string key)
        {
            ValidateKey(key);

            var item = _cache[key];

            if (item is T)
            {
                return (T)item;
            }

            if (item is Lazy<T>)
            {
                return ((Lazy<T>)item).Value;
            }

            return default(T);
        }

        public T GetOrAdd<T>(string key, Func<T> addItemFactory)
        {
            return GetOrAdd(key, addItemFactory, DefaultExpiryDateTime);
        }

        public T GetOrAdd<T>(string key, Func<T> addItemFactory, DateTimeOffset absoluteExpiration)
        {
            return GetOrAdd(key, addItemFactory, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }

        public T GetOrAdd<T>(string key, Func<T> addItemFactory, TimeSpan slidingExpiration)
        {
            return GetOrAdd(key, addItemFactory, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }

        public T GetOrAdd<T>(string key, Func<T> addItemFactory, CacheItemPolicy policy)
        {
            ValidateKey(key);

            var newLazyCacheItem = new Lazy<T>(addItemFactory);

            var existingCacheItem = _cache.AddOrGetExisting(key, newLazyCacheItem, policy);

            if (existingCacheItem != null)
            {
                if (existingCacheItem is T)
                {
                    return (T)existingCacheItem;
                }

                if (existingCacheItem is Lazy<T>)
                {
                    return ((Lazy<T>)existingCacheItem).Value;
                }

                return default(T);
            }

            try
            {
                return newLazyCacheItem.Value;
            }
            catch
            {
                // addItemFactory errored so do not cache the exception
                _cache.Remove(key);
                throw;
            }
        }

        public void Remove(string key)
        {
            ValidateKey(key);
            _cache.Remove(key);
        }

        private void ValidateKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentOutOfRangeException("key", @"Cache keys cannot be empty or whitespace");
            }
        }
    }
}