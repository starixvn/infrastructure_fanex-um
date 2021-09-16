namespace ONEbook.UM.Api.Tests.Services
{
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

        public UMTokenServiceTests()
        {
            dateTimeHelper = Substitute.For<IDateTimeHelper>();
            dateTimeHelper.GetUtcNowShortDateString().Returns("2018/01/01");
            umTokenService = new UMTokenService(dateTimeHelper);
            UnderMaintenanceApiConfiguration.Settings = new UnderMaintenanceApiConfiguration
            {
                SecretKey = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss="
            };
        }

        [Fact]
        public void IsValidUMToken_TokenIsNotValid_ReturnFalse()
        {
            // Act
            var isValid = umTokenService.IsValidUMToken(Sites._789yAgencySite, "12345");

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidUMToken_TokenIsValid_ReturnTrue()
        {
            // Act
            var isValid = umTokenService.IsValidUMToken(Sites._789yAgencySite, "dhgGhMKoWdcssjrdyo40QQ2");

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void GetUMToken_Alwasy_GenerateToken()
        {
            // Act
            var actualToken = umTokenService.GetUMToken(Sites._789yAgencySite);

            // Assert
            Assert.Equal("dhgGhMKoWdcssjrdyo40QQ2", actualToken);
        }

        [Fact]
        public void GetAllUMTokens_Alwasy_GenerateAllTokens()
        {
            // Act
            var actualTokens = umTokenService.GetAllUMTokens();

            // Assert
            Assert.Equal("dhgGhMKoWdcssjrdyo40QQ2", actualTokens[Sites._789yAgencySite].Token);
            Assert.Equal(239, actualTokens.Count);
        }
    }
}