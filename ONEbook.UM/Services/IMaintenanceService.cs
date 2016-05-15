namespace ONEbook.UM.Services
{
    using ONEbook.UM.Models;

    public interface IMaintenanceService
    {
        bool IsUnderMaintenance();

        UnderMaintenanceTime GetUnderMaintenanceTime();
    }
}