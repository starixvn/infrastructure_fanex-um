namespace ONEbook.UM.Api.Controllers
{
    using System.Web.Http;
    using Models;
    using Services;
    using Services.Impl;

    public class NewUnderMaintenanceController : ApiController
    {
        private readonly IUMControlAdapter _underMaintenanceControlAdapter;

        public NewUnderMaintenanceController()
            : this(null)
        {
        }

        public NewUnderMaintenanceController(
            IUMControlAdapter underMaintenanceControlAdapter)
        {
            _underMaintenanceControlAdapter = underMaintenanceControlAdapter ?? new NewUMControlAdapter();
        }

        [HttpGet]
        public UnderMaintenanceTime Get(int siteId)
            => _underMaintenanceControlAdapter.GetUnderMaintenanceTime(siteId);
    }
}