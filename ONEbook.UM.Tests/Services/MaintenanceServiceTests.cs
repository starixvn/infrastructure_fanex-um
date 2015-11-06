namespace ONEbook.UM.Tests.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Hosting;
    using NSubstitute;
    using NUnit.Framework;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;
    using RestSharp;
    using Sunplus.UMControllerLibrary;

    [TestFixture]
    public class MaintenanceServiceTests
    {
        private MaintenanceService _maintenanceService;
        private IUMController _underMaintenanceController;
        private IRestClient _restClient;
        private IList<string> _allowIPs;
        private string _clientIP;

        [SetUp]
        public void SetUp()
        {
            _underMaintenanceController = Substitute.For<IUMController>();
            _restClient = Substitute.For<IRestClient>();
            _allowIPs = new List<string>();
            _clientIP = "10.18.200.209";
            _maintenanceService = new MaintenanceService(_underMaintenanceController, _restClient, _allowIPs, _clientIP);
        }

        [Test]
        public void IsUnderMaintenance_ExecuteGetDowntime()
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime(false);

            _maintenanceService.IsUnderMaintenance(siteId);

            _underMaintenanceController.Received().GetUnderMaintenanceTime(
                Arg.Is<Sites>(actualSiteId => actualSiteId == siteId));
        }

        private void AssumeUMTime(bool isUMTime = true)
        {
            _underMaintenanceController.GetUnderMaintenanceTime(Arg.Any<Sites>()).Returns(new UnderMaintenanceTime
            {
                IsUnderMaintenanceTime = isUMTime
            });
        }

        [Test]
        public void IsUnderMaintenance_NowIsNotUMTime_ReturnFalse()
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime(false);

            var actualResult = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsFalse(actualResult);
        }

        private void FakeHttpContext(string queryString, bool setByPassUMToSession = false)
        {
            SimpleWorkerRequest request = new SimpleWorkerRequest(string.Empty, string.Empty, string.Empty, queryString, new StringWriter());

            HttpContext.Current = new HttpContext(request);

            if (setByPassUMToSession)
            {
                HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = true;
            }
        }

        [Test]
        public void IsUnderMaintenance_RequestDoesNotContainSecretToken_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            var queryString = string.Empty;
            FakeHttpContext(queryString);
            AssumeUMTime();

            var actualResult = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsTrue(actualResult);
        }

        [Test]
        public void IsUnderMaintenance_IPIsNotAllowedToAccess_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();

            var actualResult = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsTrue(actualResult);
        }

        [Test]
        public void IsUnderMaintenance_IPIsAllowedToAccess_ExecuteIsValidUMSecretToken()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            var actualResult = _maintenanceService.IsUnderMaintenance(siteId);

            _restClient.Received().Execute(Arg.Any<IRestRequest>());
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsNotValid_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            var actualResult = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsTrue(actualResult);
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsValid_ReturnFalse()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            var actualResult = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsFalse(actualResult);
        }
    }
}