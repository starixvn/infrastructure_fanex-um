namespace ONEbook.UM.Api.Services
{
    using Models;

    public interface IUMControlAdapter
    {
        UnderMaintenanceTime GetUnderMaintenanceTime(int siteId);
    }
}