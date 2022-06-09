namespace ONEbook.UM.Tests
{
    using System;
    using Configurations;
    using NSubstitute;
    using ONEbook.UM.Helpers;
    using UM;
    using Xunit;

    public class UMClientManagerTests
    {
        [Fact]
        public void UseConfig_ConfigIsNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => UMClientManager.UseConfig(null));
        }

        [Fact]
        public void UseConfig_ConfigIsNotNull_SetToCurrentInstance()
        {
            // Act
            var config = new UMClientConfiguration();
            UMClientManager.UseConfig(config);

            // Assert
            Assert.Equal(config, UMClientManager.UMClientConfigs);
        }

        [Fact]
        public void UseRestClient_RestClientIsNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(()
                => UMClientManager.UseConfig(new UMClientConfiguration()).UseRestClient(null));
        }

        [Fact]
        public void UseRestClient_RestClientIsNotNull_SetToCurrentInstance()
        {
            // Arrange
            var client = Substitute.For<IUMRestClient>();
            UMClientManager.UseConfig(new UMClientConfiguration()).UseRestClient(client);

            // Assert
            Assert.Equal(client, UMClientManager.RestClient);
        }

        [Fact]
        public void UseHttpContext_HttpContextIsNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(()
                => UMClientManager.UseConfig(new UMClientConfiguration()).UseHttpContext(null));
        }

        [Fact]
        public void UseHttpContext_HttpContextIsNotNull_SetToCurrentInstance()
        {
            // Arrange
            var httpContext = Substitute.For<IHttpContextHelper>();
            UMClientManager.UseConfig(new UMClientConfiguration()).UseHttpContext(httpContext);

            // Assert
            Assert.Equal(httpContext, UMClientManager.HttpContext);
        }

        [Fact]
        public void AddErrorHandler_ErrorHandlerIsNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(()
                => UMClientManager.UseConfig(new UMClientConfiguration()).AddErrorHandler(null));
        }

        [Fact]
        public void AddErrorHandler_ErrorHandlerIsNotNull_SetToProperty()
        {
            // Arrange
            UMClientManager.UseConfig(new UMClientConfiguration()).AddErrorHandler(HandleError);

            // Assert
            Assert.Equal(HandleError, UMClientManager.ErrorHandler);
        }

        private void HandleError(string message)
        {
            // Message
        }
    }
}