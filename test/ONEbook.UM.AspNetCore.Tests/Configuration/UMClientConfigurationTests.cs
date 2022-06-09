namespace ONEbook.UM.AspNetCore.Tests.Configuration
{
    using Configurations;
    using Microsoft.Extensions.Configuration;
    using Xunit;

    public class UMClientConfigurationTests
    {
        private readonly IConfiguration subConfiguration;

        public UMClientConfigurationTests()
        {
            subConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private UMClientConfiguration CreateUMClientConfiguration()
        {
            return new UMClientConfiguration().Initialize(subConfiguration);
        }

        [Fact]
        public void ServiceUri_Always_GetValueFromConfig()
        {
            // Act
            var serviceUrl = CreateUMClientConfiguration().ServiceUri;

            // Assert
            Assert.Equal("http://ha.nexdev.net:8889/api", serviceUrl);
        }

        [Fact]
        public void EnableCheckUM_Always_GetValueFromConfig()
        {
            // Act
            var enableCheckUM = CreateUMClientConfiguration().EnableCheckUM;

            // Assert
            Assert.True(enableCheckUM);
        }

        [Fact]
        public void SecretKey_Always_GetValueFromConfig()
        {
            // Act
            var secretKey = CreateUMClientConfiguration().SecretKey;

            // Assert
            Assert.Equal("9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=", secretKey);
        }

        [Fact]
        public void EnableUnscheduledUM_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().EnableUnscheduledUM;

            // Assert
            Assert.False(actualValue);
        }

        [Fact]
        public void UMFrom_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().UMFrom;

            // Assert
            Assert.Equal("3:00", actualValue);
        }

        [Fact]
        public void UMTo_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().UMTo;

            // Assert
            Assert.Equal("4:00", actualValue);
        }

        [Fact]
        public void UMPageUrl_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().UMPageUrl;

            // Assert
            Assert.Equal("/UserLogin/Common/Offline", actualValue);
        }

        [Fact]
        public void SiteId_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().SiteId;

            // Assert
            Assert.Equal(2000000, actualValue);
        }

        [Fact]
        public void ResultCacheDuration_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().ResultCacheDuration;

            // Assert
            Assert.Equal(60, actualValue);
        }

        [Fact]
        public void UMLoggingCategory_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().UMLoggingCategory;

            // Assert
            Assert.Equal("ONEbook-UM", actualValue);
        }

        [Fact]
        public void ErrorUMPageUrl_Always_GetValueFromConfig()
        {
            // Act
            var actualValue = CreateUMClientConfiguration().ErrorUMPageUrl;

            // Assert
            Assert.Equal("/Error", actualValue);
        }
    }
}