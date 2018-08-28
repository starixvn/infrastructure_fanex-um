namespace ONEbook.UM.Services
{
    using System.Threading.Tasks;
    using ONEbook.UM.Models;

    public interface IUMControlAdapter
    {
        Task<UnderMaintenanceTime> GetUnderMaintenanceTime();
    }
}