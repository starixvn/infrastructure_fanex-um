namespace ONEbook.UM.Services.Impl
{
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using Sunplus.UMControllerLibrary;

    public class UMControlAdapter : IUMControlAdapter
    {
        public UnderMaintenanceTime GetUnderMaintenanceTime(Sites siteId)
        {
            if (UMClientConfiguration.Settings.EnableUMNow)
            {
                return new UnderMaintenanceTime(true, UMClientConfiguration.Settings.UMFrom, UMClientConfiguration.Settings.UMTo);
            }

            if (UMClientConfiguration.Settings.EnableCheckUM)
            {
                string timeFrom = string.Empty;
                string timeTo = string.Empty;
                var umctrl = new UMBusiness();

                var isUnderMaintenanceTime = umctrl.isUnderMaintainence(
                    siteId,
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