namespace ONEbook.UM.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Logging;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Models;

    public interface IMaintenanceService
    {
        Task<bool> IsUnderMaintenance();

        Task<UnderMaintenanceTime> GetUnderMaintenanceTime();
    }

    public class MaintenanceService : IMaintenanceService
    {
        private const string TokenName = "UMToken";
        private const string ByPassUMSessionKey = "ONEbook.UM.ByPassUM";
        private readonly IUMControlAdapter underMaintenanceControlAdapter;
        private readonly IRestClient restClient;
        private readonly ILoggingService loggingService;
        private readonly IHttpContextHelper httpContextHelper;
        private readonly IUMClientConfiguration configuration;

        public MaintenanceService()
         : this(null, null, null, null, null, null)
        {
        }

        public MaintenanceService(
            IUMControlAdapter underMaintenanceControlAdapter,
            IRestClient restClient,
            string clientIP,
            ILoggingService loggingService,
            IHttpContextHelper httpContextHelper,
            IUMClientConfiguration configuration)
        {
            this.configuration = configuration ?? UMClientManager.UMClientConfigs;
            this.restClient = restClient ?? UMClientManager.RestClient;
            this.httpContextHelper = httpContextHelper ?? UMClientManager.HttpContext;

            this.loggingService = loggingService ?? new LoggingService(clientIP, Logger.Log, this.configuration);
            this.underMaintenanceControlAdapter = underMaintenanceControlAdapter
               ?? new UMControlAdapter(new CacheService(), this.restClient, this.configuration);
        }

        public bool ByPassUM
        {
            get
            {
                var isByPassUM = httpContextHelper.GetSession(ByPassUMSessionKey);

                return !string.IsNullOrEmpty(isByPassUM) && isByPassUM == "1";
            }

            private set
            {
                httpContextHelper.SetSession(ByPassUMSessionKey, value ? "1" : "0");
            }
        }

        public Task<UnderMaintenanceTime> GetUnderMaintenanceTime()
           => underMaintenanceControlAdapter.GetUnderMaintenanceTime();

        public async Task<bool> IsUnderMaintenance()
        {
            var nowIsNotUnderMaintenance = await NowIsNotUnderMaintenance();

            if (nowIsNotUnderMaintenance || ByPassUM)
            {
                return false;
            }

            var token = httpContextHelper.GetValueFromQueryString(TokenName);

            if (!string.IsNullOrEmpty(token) && IsTokenValid(token))
            {
                ByPassUM = true;

                return false;
            }

            return true;
        }

        private async Task<bool> NowIsNotUnderMaintenance()
        {
            var underMaintenanceTime = await underMaintenanceControlAdapter.GetUnderMaintenanceTime();

            return !underMaintenanceTime.IsUnderMaintenanceTime;
        }

        private bool IsTokenValid(string token)
        {
            var isTokenValid = false;

            try
            {
                isTokenValid = restClient.ExecuteGetAsync<bool>("/token/validate", new { configuration.SiteId, token }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                loggingService.Error(ex.Message, ex);
            }

            return isTokenValid;
        }
    }
}