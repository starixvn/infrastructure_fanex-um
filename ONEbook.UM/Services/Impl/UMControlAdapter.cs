namespace ONEbook.UM.Services.Impl
{
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using Sunplus.UMControllerLibrary;

    internal class UMControlAdapter : IUMControlAdapter
    {
        private readonly Sites _siteId;

        public UMControlAdapter()
            : this((Sites)UMClientConfiguration.Settings.SiteId)
        {
        }

        internal UMControlAdapter(Sites site)
        {
            _siteId = site;
        }

        public UnderMaintenanceTime GetUnderMaintenanceTime()
        {
            if (UMClientConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UMClientConfiguration.Settings.UMFrom, UMClientConfiguration.Settings.UMTo);
            }

            if (UMClientConfiguration.Settings.EnableCheckUM)
            {
                string timeFrom = string.Empty;
                string timeTo = string.Empty;
                var umctrl = new UMBusiness();

                var isUnderMaintenanceTime = umctrl.isUnderMaintainence(
                    _siteId,
                    UMClientConfiguration.Settings.UMAddress,
                    ref timeFrom,
                    ref timeTo);

                if (isUnderMaintenanceTime)
                {
                    return new UnderMaintenanceTime(isUnderMaintenanceTime, timeFrom, timeTo);
                }
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }
    }
}