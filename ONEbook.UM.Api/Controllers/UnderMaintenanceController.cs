namespace ONEbook.UM.Api.Controllers
{
    using System.Web.Http;
    using Models;
    using Services;
    using Services.Impl;

    public class UnderMaintenanceController : ApiController
    {
        private readonly IUMControlAdapter _umControlAdapter;

        public UnderMaintenanceController()
            : this(null)
        {
        }

        public UnderMaintenanceController(IUMControlAdapter umControlAdapter)
        {
            _umControlAdapter = umControlAdapter ?? new UMControlAdapter();
        }

        public UnderMaintenanceTime Get(int siteId)
        {
            return _umControlAdapter.GetUnderMaintenanceTime(siteId);
        }
    }
}