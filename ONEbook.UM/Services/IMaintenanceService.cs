namespace ONEbook.UM.Services
{
    using System.Threading.Tasks;
    using ONEbook.UM.Models;

    public interface IMaintenanceService
    {
        Task<bool> IsUnderMaintenance();

        Task<UnderMaintenanceTime> GetUnderMaintenanceTime();
    }
}