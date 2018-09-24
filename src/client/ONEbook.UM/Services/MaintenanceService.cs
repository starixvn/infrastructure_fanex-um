namespace ONEbook.UM.Services
{
    using System;
    using System.Collections.Generic;
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

        Task<Dictionary<int, UnderMaintenanceTime>> GetAllUnderMaintenanceTime();

        Task<Dictionary<int, UnderMaintenanceTime>> GetScheduledUnderMaintenance();
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
            var nowIsNotUnderMaintenance = await NowIsNotUnderMaintenance().ConfigureAwait(false);

            if (nowIsNotUnderMaintenance || ByPassUM)
            {
                return false;
            }

            var token = httpContextHelper.GetValueFromQueryString(TokenName);
            bool isValidToken = !string.IsNullOrEmpty(token) && await IsTokenValid(token).ConfigureAwait(false);

            if (isValidToken)
            {
                ByPassUM = true;

                return false;
            }

            return true;
        }

        public Task<Dictionary<int, UnderMaintenanceTime>> GetAllUnderMaintenanceTime()
           => restClient.ExecuteGetAsync<Dictionary<int, UnderMaintenanceTime>>("/UnderMaintenance/GetAll");

        public Task<Dictionary<int, UnderMaintenanceTime>> GetScheduledUnderMaintenance()
            => restClient.ExecuteGetAsync<Dictionary<int, UnderMaintenanceTime>>("/UnderMaintenance/GetScheduledUnderMaintenance");

        private async Task<bool> NowIsNotUnderMaintenance()
        {
            var underMaintenanceTime = await underMaintenanceControlAdapter.GetUnderMaintenanceTime().ConfigureAwait(false);

            return !underMaintenanceTime.IsUnderMaintenanceTime;
        }

        private async Task<bool> IsTokenValid(string token)
        {
            var isTokenValid = false;

            try
            {
                isTokenValid = await restClient.ExecuteGetAsync<bool>($"/token/validate?SiteId={configuration.SiteId}&Token={token}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                loggingService.Error(ex.Message, ex);

                UMClientManager.ErrorHandler?.Invoke(ex.Message);
            }

            return isTokenValid;
        }
    }
}