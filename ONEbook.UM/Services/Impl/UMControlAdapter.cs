namespace ONEbook.UM.Services.Impl
{
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Models;
    using Sunplus.UMControllerLibrary;

    public class UMControlAdapter : IUMControlAdapter
    {
        public UnderMaintenanceTime GetUnderMaintenanceTime(Sites siteId)
        {
            var downTime = new UnderMaintenanceTime
            {
                From = "0:00",
                To = "0:00",
                IsUnderMaintenanceTime = false
            };

            if (UMClientConfiguration.Settings.EnableCheckUM)
            {
                string timeFrom = string.Empty;
                string timeTo = string.Empty;
                var umctrl = new UMBusiness();

                downTime.IsUnderMaintenanceTime = umctrl.isUnderMaintainence(
                    siteId,
                    UMClientConfiguration.Settings.UMAddress,
                    ref timeFrom,
                    ref timeTo);

                if (downTime.IsUnderMaintenanceTime)
                {
                    downTime.From = timeFrom;
                    downTime.To = timeTo;
                }
            }

            return downTime;
        }
    }
}