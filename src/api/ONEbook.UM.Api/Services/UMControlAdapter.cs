namespace ONEbook.UM.Api.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Configurations;
    using Fanex.Logging;
    using Models;
    using ONELab.UMService.UMClient;
    using Sunplus.UMControllerLibrary;

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
            { 8, 3000000 },
            { 2000000, 2000000 },
            { 2000001, 2000001 },
            { 2000002, 2000002 },
            { 2000003, 2000003 },
            { 2000005, 2000005 },
            { 2000007, 2000007 },
            { 4100100, 4100300 },
        };

        private readonly UMBusiness umBusiness;

        public UMControlAdapter(UMBusiness umBusiness)
        {
            this.umBusiness = umBusiness;
        }

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
                string timeFrom = string.Empty;
                string timeTo = string.Empty;

                var isUnderMaintenanceTime = umBusiness.isUnderMaintainence(
                    (Sites)siteId,
                    UnderMaintenanceApiConfiguration.Settings.UMAddress,
                    ref timeFrom,
                    ref timeTo);

                if (isUnderMaintenanceTime)
                {
                    Logger.Log.Info(
                        $"Site: {siteId} \r\n" +
                        $"IsUM: {isUnderMaintenanceTime} \r\n" +
                        $"From: {timeFrom} \r\n" +
                        $"To: {timeTo}");

                    return new UnderMaintenanceTime(isUnderMaintenanceTime, timeFrom, timeTo);
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