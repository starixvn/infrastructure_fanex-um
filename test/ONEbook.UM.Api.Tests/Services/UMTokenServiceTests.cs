namespace ONEbook.UM.Api.Tests.Services;

using Api.Helpers;
using Api.Services;
using Configurations;
using Constants;
using Helpers;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

public class UMTokenServiceTests
{
    private readonly IUMTokenService umTokenService;

    public UMTokenServiceTests()
    {
        var dateTimeHelper = Substitute.For<IDateTimeHelper>();
        dateTimeHelper.GetUtcNowShortDateString().Returns("2018/01/01");
        var options = Substitute.For<IOptions<UnderMaintenanceApiConfiguration>>();
        options.Value.Returns(new UnderMaintenanceApiConfiguration
        {
            SecretKey = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss="
        });
        umTokenService = new UMTokenService(dateTimeHelper, options);
    }

    [Fact]
    public void IsValidUMToken_TokenIsNotValid_ReturnFalse()
    {
        // Act
        var isValid = umTokenService.IsValidUMToken(Sites.Test, "12345");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsValidUMToken_TokenIsValid_ReturnTrue()
    {
        // Act
        var isValid = umTokenService.IsValidUMToken(Sites.Test, "x3TsFOPGGunDZ8zlaCe--g");

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void GetUMToken_Alwasy_GenerateToken()
    {
        // Act
        var actualToken = umTokenService.GetUMToken(Sites.Test);

        // Assert
        Assert.Equal("x3TsFOPGGunDZ8zlaCe--g", actualToken);
    }

    [Fact]
    public void GetAllUMTokens_Alwasy_GenerateAllTokens()
    {
        // Act
        var actualTokens = umTokenService.GetAllUMTokens();

        // Assert
        Assert.Equal("x3TsFOPGGunDZ8zlaCe--g", actualTokens[Sites.Test].Token);
        Assert.Equal(17, actualTokens.Count);
    }
}