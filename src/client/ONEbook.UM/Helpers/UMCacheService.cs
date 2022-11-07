namespace ONEbook.UM.Helpers
{
    using System;

    public interface IUMCacheService
    {
        T Get<T>(string key);

        void Set<T>(string key, T value, TimeSpan absoluteExpiration);

        void Remove(string key);
    }
}