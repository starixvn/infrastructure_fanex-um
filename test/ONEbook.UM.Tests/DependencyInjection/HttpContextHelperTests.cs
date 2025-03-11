using Xunit;

namespace ONEbook.UM.Tests.DependencyInjection;

using Microsoft.AspNetCore.Http;
using NSubstitute;
using UM.DependencyInjection;
using UM.Helpers;

public class HttpContextHelperTests
{
    private readonly IHttpContextHelper httpContextHelper;
    public HttpContextHelperTests()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextHelper = new HttpContextHelper(httpContextAccessor);
    }

    [Fact]
    public void GetSession_HttpContextAccessorIsNull_ReturnEmpty()
    {
        // Act
        var actual = httpContextHelper.GetSession("key");
        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public void SetSession_HttpContextAccessorIsNull_ReturnEmpty()
    {
        // Act
        httpContextHelper.SetSession("key", "value");
        // Assert
        Assert.True(true);
    }

    [Fact]
    public void GetValueFromQueryString_HttpContextAccessorIsNull_ReturnEmpty()
    {
        // Act
        var actual = httpContextHelper.GetValueFromQueryString("key");
        // Assert
        Assert.Equal(string.Empty, actual);
    }
}

