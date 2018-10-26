namespace ONEbook.UM.AspNetMVC.Tests.Configuration
{
    using NSubstitute;
    using ONEbook.UM.AspNetMVC.Configuration;
    using System;
    using System.Configuration;
    using Xunit;

    public class UMClientConfigurationTests
    {
        public UMClientConfigurationTests()
        {
        }

        private UMClientConfiguration CreateUMClientConfiguration()
        {
            return new UMClientConfiguration();
        }

        [Fact]
        public void ServiceUri_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:ServiceUri"] = "http://msite.starific.net/v5/api";

            // Act
            var serviceUrl = CreateUMClientConfiguration().ServiceUri;

            // Assert
            Assert.Equal("http://msite.starific.net/v5/api", serviceUrl);
        }

        [Fact]
        public void EnableCheckUM_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:EnableCheckUM"] = "true";

            // Act
            var enableCheckUM = CreateUMClientConfiguration().EnableCheckUM;

            // Assert
            Assert.True(enableCheckUM);
        }

        [Fact]
        public void SecretKey_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:SecretKey"] = "12345";

            // Act
            var secretKey = CreateUMClientConfiguration().SecretKey;

            // Assert
            Assert.Equal("12345", secretKey);
        }

        [Fact]
        public void EnableUnscheduledUM_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:EnableUnscheduledUM"] = "true";

            // Act
            var actualValue = CreateUMClientConfiguration().EnableUnscheduledUM;

            // Assert
            Assert.True(actualValue);
        }

        [Fact]
        public void UMFrom_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:UMFrom"] = "3:00";

            // Act
            var actualValue = CreateUMClientConfiguration().UMFrom;

            // Assert
            Assert.Equal("3:00", actualValue);
        }

        [Fact]
        public void UMTo_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:UMTo"] = "3:00";

            // Act
            var actualValue = CreateUMClientConfiguration().UMTo;

            // Assert
            Assert.Equal("3:00", actualValue);
        }

        [Fact]
        public void UMPageUrl_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:UMPageUrl"] = "12345";

            // Act
            var actualValue = CreateUMClientConfiguration().UMPageUrl;

            // Assert
            Assert.Equal("12345", actualValue);
        }

        [Fact]
        public void SiteId_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:SiteId"] = "12345";

            // Act
            var actualValue = CreateUMClientConfiguration().SiteId;

            // Assert
            Assert.Equal(12345, actualValue);
        }

        [Fact]
        public void ResultCacheDuration_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:ResultCacheDuration"] = "60";

            // Act
            var actualValue = CreateUMClientConfiguration().ResultCacheDuration;

            // Assert
            Assert.Equal(60, actualValue);
        }

        [Fact]
        public void UMLoggingCategory_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:UMLoggingCategory"] = "UM";

            // Act
            var actualValue = CreateUMClientConfiguration().UMLoggingCategory;

            // Assert
            Assert.Equal("UM", actualValue);
        }

        [Fact]
        public void ErrorUMPageUrl_Always_GetValueFromConfig()
        {
            // Arrange
            ConfigurationManager.AppSettings["ONEbook.UM:ErrorUMPageUrl"] = "UM";

            // Act
            var actualValue = CreateUMClientConfiguration().ErrorUMPageUrl;

            // Assert
            Assert.Equal("UM", actualValue);
        }
    }
}