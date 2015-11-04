namespace ONEbook.UM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sunplus.UMControllerLibrary;

    public interface IMaintenanceService
    {
        bool IsUnderMaintenance(Sites siteId);

        bool IsUnderMaintenanceOnReCheckIn(Sites siteId);
    }
}
