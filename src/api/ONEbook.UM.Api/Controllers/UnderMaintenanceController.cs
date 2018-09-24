namespace ONEbook.UM.Api.Controllers
{
    using System.Collections.Generic;
    using System.Web.Http;
    using Models;
    using Services;

    public class UnderMaintenanceController : ApiController
    {
        private readonly IUMControlAdapter underMaintenanceControlAdapter;

        public UnderMaintenanceController()
            : this(null)
        {
        }

        public UnderMaintenanceController(IUMControlAdapter underMaintenanceControlAdapter)
        {
            this.underMaintenanceControlAdapter = underMaintenanceControlAdapter ?? new UMControlAdapter();
        }

        public UnderMaintenanceTime Get(int siteId)
            => underMaintenanceControlAdapter.GetUnderMaintenanceTime(siteId);

        public Dictionary<int, UnderMaintenanceTime> GetAll()
             => underMaintenanceControlAdapter.GetAllUnderMaintenanceTime();

        public Dictionary<int, UnderMaintenanceTime> GetScheduledUnderMaintenance()
            => underMaintenanceControlAdapter.GetScheduledUnderMaintenanceInformation();
    }
}