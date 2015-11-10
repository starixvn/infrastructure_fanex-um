namespace ONEbook.UM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sunplus.UMControllerLibrary;

    public interface IMaintenanceService
    {
        bool IsUnderMaintenanceOnLogin(Sites siteId);

        bool IsUnderMaintenanceOnRecheckIn(Sites siteId);
    }
}
