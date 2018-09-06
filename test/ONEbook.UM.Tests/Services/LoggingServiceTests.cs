namespace ONEbook.UM.Tests.Services
{
    using System;
    using Fanex.Logging;
    using NSubstitute;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using Xunit;

    public class LoggingServiceTests
    {
        private readonly ILogger logger;
        private readonly IPHelper ipHelper;
        private readonly IUMClientConfiguration configuration;
        private ILoggingService loggingService;

        public LoggingServiceTests()
        {
            logger = Substitute.For<ILogger>();
            ipHelper = Substitute.For<IPHelper>();
            configuration = Substitute.For<IUMClientConfiguration>();
        }

        [Fact]
        public void Error_Always_LogErrorMessage()
        {
            // Arrange
            configuration.UMLoggingCategory.Returns("LogCategory");
            configuration.SiteId.Returns(10000);
            loggingService = new LoggingService("127.0.0.1", logger, configuration, ipHelper);

            // Act
            loggingService.Error("Error");

            // Assert
            logger.Received().Error(Arg.Is("Message: Error\r\nSiteId: 10000\r\nClientIP: 127.0.0.1"), Arg.Is("LogCategory"));
        }

        [Fact]
        public void Error_WithException_LogErrorMessage()
        {
            // Arrange
            configuration.UMLoggingCategory.Returns("LogCategory");
            configuration.SiteId.Returns(10000);
            loggingService = new LoggingService("127.0.0.1", logger, configuration, ipHelper);
            var exception = new Exception();

            // Act
            loggingService.Error("Error", exception);

            // Assert
            logger.Received().Error(Arg.Is("Message: Error\r\nSiteId: 10000\r\nClientIP: 127.0.0.1"), Arg.Is("LogCategory"), Arg.Is(exception));
        }

        [Fact]
        public void Warning_Always_LogWarningMessage()
        {
            // Arrange
            configuration.UMLoggingCategory.Returns("LogCategory");
            configuration.SiteId.Returns(10000);
            loggingService = new LoggingService("127.0.0.1", logger, configuration, ipHelper);

            // Act
            loggingService.Warning("Error");

            // Assert
            logger.Received().Warn(Arg.Is("Message: Error\r\nSiteId: 10000\r\nClientIP: 127.0.0.1"), Arg.Is("LogCategory"));
        }
    }
}