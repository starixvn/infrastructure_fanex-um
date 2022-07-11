namespace ONEbook.UM.Api.Tests.Services
{
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using ONEbook.UM.Api.Configurations;
    using ONEbook.UM.Api.Constants;
    using ONEbook.UM.Api.Helpers;
    using ONEbook.UM.Api.Services;
    using Xunit;

    public class UMTokenServiceTests
    {
        private readonly IUMTokenService umTokenService;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IOptions<UnderMaintenanceApiConfiguration> options;

        public UMTokenServiceTests()
        {
            dateTimeHelper = Substitute.For<IDateTimeHelper>();
            dateTimeHelper.GetUtcNowShortDateString().Returns("2018/01/01");
            options = Substitute.For<IOptions<UnderMaintenanceApiConfiguration>>();
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
            var isValid = umTokenService.IsValidUMToken(Sites.Test, "x3TsFOPGGunDZ8zlaCe--g2");

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void GetUMToken_Alwasy_GenerateToken()
        {
            // Act
            var actualToken = umTokenService.GetUMToken(Sites.Test);

            // Assert
            Assert.Equal("x3TsFOPGGunDZ8zlaCe--g2", actualToken);
        }

        [Fact]
        public void GetAllUMTokens_Alwasy_GenerateAllTokens()
        {
            // Act
            var actualTokens = umTokenService.GetAllUMTokens();

            // Assert
            Assert.Equal("x3TsFOPGGunDZ8zlaCe--g2", actualTokens[Sites.Test].Token);
            Assert.Equal(14, actualTokens.Count);
        }
    }
}