namespace ONEbook.UM.Api.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Configurations;
    using Fanex.Logging;
    using Models;
    using ONEbook.UM.Api.Constants;
    using ONELab.UMService.UMClient;

    public interface IUMControlAdapter
    {
        UnderMaintenanceTime GetUnderMaintenanceTime(int siteId);

        Dictionary<int, UnderMaintenanceTime> GetScheduledUnderMaintenanceInformation();

        Dictionary<int, UnderMaintenanceTime> GetAllUnderMaintenanceTime();
    }

    public class UMControlAdapter : IUMControlAdapter
    {
        /// <summary>
        /// Old and New SiteID for UM Tool
        /// </summary>
        private static readonly IDictionary<int, int> OldNewSiteIdMappers = new Dictionary<int, int>
        {
            { (int)Sites.MBCAgencySite, (int)Sites.MBCAgencySite },
            { (int)Sites.IndoAgencySite, (int)Sites.IndoAgencySite },
            { (int)Sites._789yAgencySite, (int)Sites._789yAgencySite },
            { (int)Sites.AccountSite, (int)Sites.AccountSite },
            { (int)Sites.LRF, (int)Sites.LRF },
            { (int)Sites.OWA, (int)Sites.OWA },
            { (int)Sites.Aff12Bet, (int)Sites.Aff12Bet },
            { (int)Sites.RMS, (int)Sites.RMS },
            { (int)Sites.OddsFeed, (int)Sites.OddsFeed },
            { (int)Sites._789y, 4100300 },
            { (int)Sites.Test, (int)Sites.Test },
        };

        public UnderMaintenanceTime GetUnderMaintenanceTime(int siteId)
        {
            var fakeUMInfo = FakeUMControlAdapter.GetUnderMaintenance();

            if (fakeUMInfo != null)
            {
                return fakeUMInfo;
            }

            if (UnderMaintenanceApiConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UnderMaintenanceApiConfiguration.Settings.UMFrom, UnderMaintenanceApiConfiguration.Settings.UMTo);
            }

            if (UnderMaintenanceApiConfiguration.Settings.EnableCheckUM)
            {
                var isUnderUM = ServiceClient.IsUnderUM(siteId.ToString(), out DateTime dateFrom, out DateTime dateTo, out int errorCode);
                if (errorCode != Clients.SUCCESS_ECODE && errorCode != Clients.NO_FOUND_ECODE)
                {
                    Logger.Log.Error($"Service errorCode: {errorCode}, siteId:{siteId}");
                    return new UnderMaintenanceTime(false, "0:00", "0:00");
                }

                if (isUnderUM)
                {
                    var timeFrom = dateFrom.ToString("HH:mm");
                    var timeTo = dateTo.ToString("HH:mm");
                    Logger.Log.Info(
                        $"Site: {siteId} \r\n" +
                        $"From: {timeFrom} \r\n" +
                        $"To: {timeTo}");

                    return new UnderMaintenanceTime(isUnderUM, timeFrom, timeTo);
                }
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }

        public Dictionary<int, UnderMaintenanceTime> GetAllUnderMaintenanceTime()
            => OldNewSiteIdMappers.Keys.ToDictionary(siteId => siteId, GetUnderMaintenanceTime);

        public Dictionary<int, UnderMaintenanceTime> GetScheduledUnderMaintenanceInformation()
        {
            var siteIds = OldNewSiteIdMappers.Keys;

            var fakeUMInfo = FakeUMControlAdapter.GetUnderMaintenance();

            if (fakeUMInfo != null)
            {
                return siteIds.ToDictionary(siteId => siteId, _ => fakeUMInfo);
            }

            return siteIds.ToDictionary(siteId => siteId, siteId => GetScheduledUnderMaintenance(OldNewSiteIdMappers[siteId]));
        }

        private static UnderMaintenanceTime GetScheduledUnderMaintenance(int siteId)
        {
            if (UnderMaintenanceApiConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UnderMaintenanceApiConfiguration.Settings.UMFrom, UnderMaintenanceApiConfiguration.Settings.UMTo);
            }

            var isUnderUM = ServiceClient.IsUnderUM(siteId.ToString(), out DateTime startTime, out DateTime endTime, out int errorCode);

            if (startTime.Date.Year != DateTime.MinValue.Year)
            {
                return new UnderMaintenanceTime(
                   isUnderUM,
                   startTime.ToString(CultureInfo.InvariantCulture),
                   endTime.ToString(CultureInfo.InvariantCulture),
                   errorCode);
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }
    }
}