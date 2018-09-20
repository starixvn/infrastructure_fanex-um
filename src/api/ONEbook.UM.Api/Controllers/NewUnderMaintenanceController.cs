namespace ONEbook.UM.Api.Controllers
{
    using System;
    using System.Configuration;
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
            var useOldUM = Convert.ToBoolean(ConfigurationManager.AppSettings["UseOldUM"] ?? "false");
            IUMControlAdapter umControlAdapterInstance = useOldUM ?
                new UMControlAdapter() as IUMControlAdapter :
                new NewUMControlAdapter();

            _underMaintenanceControlAdapter = underMaintenanceControlAdapter ?? umControlAdapterInstance;
        }

        [HttpGet]
        public UnderMaintenanceTime Get(int siteId)
            => _underMaintenanceControlAdapter.GetUnderMaintenanceTime(siteId);
    }
}