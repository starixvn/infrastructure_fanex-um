namespace ONEbook.UM.Services.Impl
{
    using System;
    using Configurations;
    using Fanex.Logging;
    using Helpers;

    public class LoggingService : ILoggingService
    {
        private readonly string _clientIP;
        private readonly int _siteId;
        private readonly ILogger _logger;
        private readonly string _logCategory;

        public LoggingService()
            : this(null, 0, Logger.Log, null)
        {
        }

        internal LoggingService(string clientIP, int siteId, ILogger logger, string logCategory = null)
        {
            _clientIP = clientIP;
            _siteId = siteId == 0 ? UMClientConfiguration.Settings.SiteId : siteId;
            _logger = logger ?? Logger.Log;
            _logCategory = logCategory ?? UMClientConfiguration.Settings.UMLoggingCategory;
        }

        public void Error(string message)
        {
            _logger.Error(BuildMessage(message), _logCategory);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(BuildMessage(message), _logCategory, exception);
        }

        private string BuildMessage(string message)
        {
            return string.Join(
                "\r\n",
                "Message: " + message,
                "SiteId: " + _siteId,
                "ClientIP: " + (_clientIP ?? IPHelper.GetMachineIP()));
        }

        public void Warning(string message)
        {
            _logger.Warn(BuildMessage(message), _logCategory);
        }
    }
}