namespace ONEbook.UM.Api.Tests.Services
{
    using NSubstitute;
    using ONEbook.UM.Api.Configurations;
    using ONEbook.UM.Api.Helpers;
    using ONEbook.UM.Api.Services;
    using Sunplus.UMControllerLibrary;
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
            var isValid = umTokenService.IsValidUMToken(Sites.ABB688, "12345");

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void IsValidUMToken_TokenIsValid_ReturnTrue()
        {
            // Act
            var isValid = umTokenService.IsValidUMToken(Sites.ABB688, "dhgGhMKoWdcssjrdyo40QQ2");

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void GetUMToken_Alwasy_GenerateToken()
        {
            // Act
            var actualToken = umTokenService.GetUMToken(Sites.ABB688);

            // Assert
            Assert.Equal("dhgGhMKoWdcssjrdyo40QQ2", actualToken);
        }

        [Fact]
        public void GetAllUMTokens_Alwasy_GenerateAllTokens()
        {
            // Act
            var actualTokens = umTokenService.GetAllUMTokens();

            // Assert
            Assert.Equal("dhgGhMKoWdcssjrdyo40QQ2", actualTokens[Sites.ABB688].Token);
            Assert.Equal(239, actualTokens.Count);
        }
    }
}