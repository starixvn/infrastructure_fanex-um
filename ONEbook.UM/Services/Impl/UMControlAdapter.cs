namespace ONEbook.UM.Services.Impl
{
    using System;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using ONELab.UMService.UMClient;

    internal class UMControlAdapter : IUMControlAdapter
    {
        private readonly int _siteId;

        public UMControlAdapter()
            : this(UMClientConfiguration.Settings.SiteId)
        {
        }

        internal UMControlAdapter(int siteId)
        {
            _siteId = siteId;
        }

        public UnderMaintenanceTime GetUnderMaintenanceTime()
        {
            if (UMClientConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UMClientConfiguration.Settings.UMFrom, UMClientConfiguration.Settings.UMTo);
            }

            if (UMClientConfiguration.Settings.EnableCheckUM)
            {
                DateTime timeFrom;
                DateTime timeTo;

                var isUnderMaintenanceTime = ServiceClient.IsUnderUM(
                    _siteId.ToString(),
                    out timeFrom,
                    out timeTo);

                if (isUnderMaintenanceTime)
                {
                    return new UnderMaintenanceTime(isUnderMaintenanceTime, timeFrom.ToString(), timeTo.ToString());
                }
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }
    }
}