using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONEbook.UM.Api.Tests.Helpers;

using Api.Helpers;
using Xunit;

// Render unit tests for all method of DateTimeHelper
public class DateTimeHelperTests
{
    [Fact]
    public void GetUtcNowShortDateStringTest()
    {
        // Arrange
        var dateTimeHelper = new DateTimeHelper();
        // Act
        var result = dateTimeHelper.GetUtcNowShortDateString();
        // Assert
        Assert.NotNull(result);
    }
}