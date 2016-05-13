namespace ONEbook.UM.Api.Services.Impl
{
    using System;
    using Configurations;
    using Models;
    using ONELab.UMService.UMClient;

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
                DateTime timeFrom;
                DateTime timeTo;

                var isUnderMaintenanceTime = ServiceClient.IsUnderUM(
                    siteId.ToString(),
                    out timeFrom,
                    out timeTo);

                if (isUnderMaintenanceTime)
                {
                    return new UnderMaintenanceTime(isUnderMaintenanceTime, timeFrom.ToString(FormatTime), timeTo.ToString(FormatTime));
                }
            }

            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }
    }
}