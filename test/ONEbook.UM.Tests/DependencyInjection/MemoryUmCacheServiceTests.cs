using System;

namespace ONEbook.UM.Tests.DependencyInjection;

using Microsoft.Extensions.Caching.Memory;
using UM.DependencyInjection;
using Xunit;

public class MemoryUmCacheServiceTests
{
    private IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public void Get_Always_ReturnValue()
    {
        // Arrange
        var service = new MemoryUmCacheService(memoryCache);

        // Act  
        service.Set("key", "value", TimeSpan.FromDays(1));
        var actual = service.Get<string>("key");

        // Assert
        Assert.Equal("value", actual);

    }

    [Fact]
    public void Set_Always_SetValue()
    {
        // Arrange
        var service = new MemoryUmCacheService(memoryCache);
        // Act
        service.Set("key", "value", TimeSpan.FromDays(1));

    }

    [Fact]
    public void Remove_Always_RemoveValue()
    {
        // Arrange
        var service = new MemoryUmCacheService(memoryCache);

        // Act
        service.Set("removed_key", "value", TimeSpan.FromDays(1));
        service.Remove("removed_key");
        var actual = service.Get<string>("removed_key");

        // Assert
        Assert.Null(actual);
    }
}