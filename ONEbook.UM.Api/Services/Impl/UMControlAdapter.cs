namespace ONEbook.UM.Api.Services.Impl
{
    using Configurations;
    using Models;
    using Sunplus.UMControllerLibrary;

    public class UMControlAdapter : IUMControlAdapter
    {
        private const string FormatTime = "HH:mm tt";

        public UnderMaintenanceTime GetUnderMaintenanceTime(int siteId)
        {
            if (UnderMaintenanceApiConfiguration.Settings.EnableUnscheduledUM)
            {
                return new UnderMaintenanceTime(true, UnderMaintenanceApiConfiguration.Settings.UMFrom, UnderMaintenanceApiConfiguration.Settings.UMTo);
            }

            if (UnderMaintenanceApiConfiguration.Settings.EnableCheckUM)
            {
                string timeFrom = string.Empty;
                string timeTo = string.Empty;
                var umctrl = new UMBusiness();

                var isUnderMaintenanceTime = umctrl.isUnderMaintainence(
                    (Sites)siteId,
                    UnderMaintenanceApiConfiguration.Settings.UMAddress,
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