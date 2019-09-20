namespace ONEbook.UM.AspNetCore.Tests.Configuration
{
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using Configurations;
    using Xunit;

    public class UMClientConfigurationTests
    {
        private readonly IConfiguration subConfiguration;

        public UMClientConfigurationTests()
        {
            subConfiguration = Substitute.For<IConfiguration>();
        }

        private UMClientConfiguration CreateUMClientConfiguration()
        {
            return new UMClientConfiguration();
        }

        [Fact]
        public void ServiceUri_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:ServiceUri"].Returns("http://www.google.com");

            // Act
            var serviceUrl = CreateUMClientConfiguration().ServiceUri;

            // Assert
            Assert.Equal("http://www.google.com", serviceUrl);
        }

        [Fact]
        public void EnableCheckUM_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:EnableCheckUM"].Returns("true");

            // Act
            var enableCheckUM = CreateUMClientConfiguration().EnableCheckUM;

            // Assert
            Assert.True(enableCheckUM);
        }

        [Fact]
        public void SecretKey_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:SecretKey"].Returns("12345");

            // Act
            var secretKey = CreateUMClientConfiguration().SecretKey;

            // Assert
            Assert.Equal("12345", secretKey);
        }

        [Fact]
        public void EnableUnscheduledUM_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:EnableUnscheduledUM"].Returns("true");

            // Act
            var actualValue = CreateUMClientConfiguration().EnableUnscheduledUM;

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void UMFrom_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:UMFrom"].Returns("3:00");

            // Act
            var actualValue = CreateUMClientConfiguration().UMFrom;

            // Assert
            Assert.Equal("3:00", actualValue);
        }

        [Fact]
        public void UMTo_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:UMTo"].Returns("3:00");

            // Act
            var actualValue = CreateUMClientConfiguration().UMTo;

            // Assert
            Assert.Equal("3:00", actualValue);
        }

        [Fact]
        public void UMPageUrl_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:UMPageUrl"].Returns("12345");

            // Act
            var actualValue = CreateUMClientConfiguration().UMPageUrl;

            // Assert
            Assert.Equal("12345", actualValue);
        }

        [Fact]
        public void SiteId_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:SiteId"].Returns("12345");

            // Act
            var actualValue = CreateUMClientConfiguration().SiteId;

            // Assert
            Assert.Equal(12345, actualValue);
        }

        [Fact]
        public void ResultCacheDuration_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:ResultCacheDuration"].Returns("60");

            // Act
            var actualValue = CreateUMClientConfiguration().ResultCacheDuration;

            // Assert
            Assert.Equal(60, actualValue);
        }

        [Fact]
        public void UMLoggingCategory_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:UMLoggingCategory"].Returns("UM");

            // Act
            var actualValue = CreateUMClientConfiguration().UMLoggingCategory;

            // Assert
            Assert.Equal("UM", actualValue);
        }

        [Fact]
        public void ErrorUMPageUrl_Always_GetValueFromConfig()
        {
            // Arrange
            subConfiguration["ONEbook.UM:ErrorUMPageUrl"].Returns("UM");

            // Act
            var actualValue = CreateUMClientConfiguration().ErrorUMPageUrl;

            // Assert
            Assert.Equal("UM", actualValue);
        }
    }
}