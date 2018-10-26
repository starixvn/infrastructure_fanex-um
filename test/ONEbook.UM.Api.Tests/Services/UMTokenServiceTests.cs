using NSubstitute;
using ONEbook.UM.Api.Configurations;
using ONEbook.UM.Api.Services;
using Sunplus.UMControllerLibrary;
using System;
using Xunit;

namespace ONEbook.UM.Api.Tests.Services
{
    public class UMTokenServiceTests
    {
        private readonly IUMTokenService umTokenService;

        public UMTokenServiceTests()
        {
            umTokenService = new UMTokenService();
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
            var isValid = umTokenService.IsValidUMToken(Sites.ABB688, "m1LlAeG78SkWG_Yhe8-rTQ2");

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void GetUMToken_Alwasy_GenerateToken()
        {
            // Act
            var actualToken = umTokenService.GetUMToken(Sites.ABB688);

            // Assert
            Assert.Equal("m1LlAeG78SkWG_Yhe8-rTQ2", actualToken);
        }

        [Fact]
        public void GetAllUMTokens_Alwasy_GenerateAllTokens()
        {
            // Act
            var actualTokens = umTokenService.GetAllUMTokens();

            // Assert
            Assert.Equal("m1LlAeG78SkWG_Yhe8-rTQ2", actualTokens[Sites.ABB688].Token);
            Assert.Equal("pWijipbAo67nlPGMptd_eQ2", actualTokens[Sites.ABcasino].Token);
            Assert.Equal(239, actualTokens.Count);
        }
    }
}