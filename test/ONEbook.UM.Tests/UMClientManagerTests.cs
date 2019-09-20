namespace ONEbook.UM.Tests
{
    using NSubstitute;
    using ONEbook.UM;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using System;
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
            var config = Substitute.For<UMClientConfiguration>();
            UMClientManager.UseConfig(config);

            // Assert
            Assert.Equal(config, UMClientManager.UMClientConfigs);
        }

        [Fact]
        public void UseRestClient_RestClientIsNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => UMClientManager.UseConfig(Substitute.For<UMClientConfiguration>()).UseRestClient(null));
        }

        [Fact]
        public void UseRestClient_RestClientIsNotNull_SetToCurrentInstance()
        {
            // Arrange
            var client = Substitute.For<IUMRestClient>();
            UMClientManager.UseConfig(Substitute.For<UMClientConfiguration>()).UseRestClient(client);

            // Assert
            Assert.Equal(client, UMClientManager.RestClient);
        }

        [Fact]
        public void UseHttpContext_HttpContextIsNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => UMClientManager.UseConfig(Substitute.For<UMClientConfiguration>()).UseHttpContext(null));
        }

        [Fact]
        public void UseHttpContext_HttpContextIsNotNull_SetToCurrentInstance()
        {
            // Arrange
            var httpContext = Substitute.For<IHttpContextHelper>();
            UMClientManager.UseConfig(Substitute.For<UMClientConfiguration>()).UseHttpContext(httpContext);

            // Assert
            Assert.Equal(httpContext, UMClientManager.HttpContext);
        }

        [Fact]
        public void AddErrorHandler_ErrorHandlerIsNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => UMClientManager.UseConfig(Substitute.For<UMClientConfiguration>()).AddErrorHandler(null));
        }

        [Fact]
        public void AddErrorHandler_ErrorHandlerIsNotNull_SetToProperty()
        {
            // Arrange
            UMClientManager.UseConfig(Substitute.For<UMClientConfiguration>()).AddErrorHandler(HandleError);

            // Assert
            Assert.Equal(HandleError, UMClientManager.ErrorHandler);
        }

        private void HandleError(string message)
        {
            // Message
        }
    }
}