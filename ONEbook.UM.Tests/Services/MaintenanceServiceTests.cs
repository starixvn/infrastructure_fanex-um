namespace ONEbook.UM.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using NSubstitute;
    using NUnit.Framework;
    using Sunplus.UMControllerLibrary;

    [TestFixture]
    public class MaintenanceServiceTests
    {
        private MaintenanceService _maintenanceService;
        private IUMController _umController;

        [SetUp]
        public void SetUp()
        {
            _umController = Substitute.For<IUMController>();
            _maintenanceService = new MaintenanceService(_umController);
        }

        [Test]
        public void IsUnderMaintenance_ExecuteGetDowntime()
        {
            var siteId = Sites.AccountSite;

            _maintenanceService.IsUnderMaintenance(siteId);

            _umController.Received().GetUnderMaintenanceTime(
                Arg.Is<Sites>( actualSiteId => actualSiteId == siteId));
        }

        ////[Test]
        ////public void IsUnderMaintenance_ExecuteGetDowntime
    }
}
