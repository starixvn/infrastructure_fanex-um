namespace ONEbook.UM.Api.Services.Impl
{
    using System;
    using ONEbook.UM.Api.Models;
    using ONELab.UMService.UMClient;
    using ONEbook.UM.Api.Extensions;

    public class NewUMControlAdapter : IUMControlAdapter
    {
        public UnderMaintenanceTime GetUnderMaintenanceTime(int siteId)
        {
            var fakeUMInfo = FakeUMControlAdapter.GetUnderMaintenance();

            if (fakeUMInfo != null)
            {
                return fakeUMInfo;
            }

            var isUnderUM = ServiceClient.IsUnderUM(siteId.ToString(), out DateTime startTime, out DateTime endTime, out int errorCode);

            return new UnderMaintenanceTime(isUnderUM, startTime.ToString(), endTime.ToString(), errorCode);
        }
    }
}