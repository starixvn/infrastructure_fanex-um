namespace ONEbook.UM.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Logging;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Models;

    public class MaintenanceService : IMaintenanceService
    {
        private const string TokenName = "UMToken";
        private const string ByPassUMSessionKey = "ONEbook.UM.ByPassUM";
        private readonly IUMControlAdapter underMaintenanceControlAdapter;
        private readonly IRestClient restClient;
        private readonly ILoggingService loggingService;
        private readonly IList<Tuple<string, string>> allowedIPs;
        private readonly string clientIP;
        private readonly int siteId;
        private readonly bool isAllowAllIPs;
        private readonly IHttpContextHelper httpContextHelper;

        public MaintenanceService(IRestClient restClient, IHttpContextHelper httpContextHelper)
         : this(null, restClient, null, null, null, 0, null, httpContextHelper)
        {
        }

        internal MaintenanceService(
            IUMControlAdapter underMaintenanceControlAdapter,
            IRestClient restClient,
            IList<Tuple<string, string>> allowedIPs,
            string clientIP,
            ILoggingService loggingService,
            int siteId,
            bool? isAllowAllIPs,
            IHttpContextHelper httpContextHelper)
        {
            this.restClient = restClient;
            this.siteId = siteId == 0 ? UMClientConfiguration.Settings.SiteId : siteId;
            this.loggingService = loggingService ?? new LoggingService(clientIP, this.siteId, Logger.Log, UMClientConfiguration.Settings.UMLoggingCategory);
            this.underMaintenanceControlAdapter = underMaintenanceControlAdapter ?? new UMControlAdapter(new CacheService(), this.restClient);
            this.allowedIPs = allowedIPs ?? UMClientConfiguration.AllowedIPs;
            this.clientIP = clientIP;
            this.isAllowAllIPs = isAllowAllIPs.HasValue ? isAllowAllIPs.Value : UMClientConfiguration.Settings.IPList.AllowAll;
            this.httpContextHelper = httpContextHelper;
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

            if (!string.IsNullOrEmpty(token)
                && IsRequestIPAllowToAccess()
                && IsTokenValid(token))
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
                isTokenValid = restClient.ExecuteGetAsync<bool>("/token/validate", new { siteId, token }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                loggingService.Error(ex.Message, ex);
            }

            return isTokenValid;
        }

        private bool IsRequestIPAllowToAccess()
        {
            if (isAllowAllIPs)
            {
                return true;
            }

            return IPHelper.IsAuthorizedIP(this.clientIP ?? httpContextHelper.GetIpAddress(), allowedIPs);
        }
    }
}