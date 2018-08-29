namespace ONEbook.UM.Services
{
    using System;
    using Configurations;
    using Fanex.Logging;
    using Helpers;

    public interface ILoggingService
    {
        void Error(string message);

        void Error(string message, Exception exception);

        void Warning(string message);
    }

    public class LoggingService : ILoggingService
    {
        private readonly string clientIP;
        private readonly int siteId;
        private readonly ILogger logger;
        private readonly string logCategory;
        private readonly IPHelper ipHelper;

        public LoggingService()
            : this(null, Logger.Log, null)
        {
        }

        internal LoggingService(string clientIP, ILogger logger, IUMClientConfiguration configuration, IPHelper ipHelper = null)
        {
            this.clientIP = clientIP;
            siteId = configuration.SiteId;
            this.logger = logger ?? Logger.Log;
            logCategory = configuration.UMLoggingCategory;
            this.ipHelper = ipHelper ?? new IPHelperImpl();
        }

        public void Error(string message) => logger.Error(BuildMessage(message), logCategory);

        public void Error(string message, Exception exception) => logger.Error(BuildMessage(message), logCategory, exception);

        private string BuildMessage(string message)
            => string.Join(
                "\r\n",
                "Message: " + message,
                "SiteId: " + siteId,
                "ClientIP: " + (clientIP ?? ipHelper.GetMachineIP()));

        public void Warning(string message) => logger.Warn(BuildMessage(message), logCategory);
    }
}