namespace ONEbook.UM.Services
{
    using ONEbook.UM.Models;
    using Sunplus.UMControllerLibrary;

    public interface IUMControlAdapter
    {
        UnderMaintenanceTime GetUnderMaintenanceTime();
    }
}