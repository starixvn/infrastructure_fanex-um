namespace ONEbook.UM.Tests.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.SessionState;
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

        #region IsUnderMaintenance Test cases

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

            var isUM = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsFalse(isUM);
        }

        private void FakeHttpContext(string queryString, bool setByPassUMToSession = false)
        {
            SimpleWorkerRequest request = new SimpleWorkerRequest(string.Empty, string.Empty, string.Empty, queryString, new StringWriter());

            HttpContext.Current = new HttpContext(request);

            var sessionContainer = new HttpSessionStateContainer(
                                                "id",
                                               new SessionStateItemCollection(),
                                               new HttpStaticObjectsCollection(),
                                               10,
                                               true,
                                               HttpCookieMode.AutoDetect,
                                               SessionStateMode.InProc,
                                               false);
            HttpContext.Current.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    null,
                                    CallingConventions.Standard,
                                    new[] { typeof(HttpSessionStateContainer) },
                                    null)
                .Invoke(new object[] { sessionContainer });
        }

        [Test]
        public void IsUnderMaintenance_RequestDoesNotContainSecretToken_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            var queryString = string.Empty;
            FakeHttpContext(queryString);
            AssumeUMTime();

            var isUM = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenance_IPIsNotAllowedToAccess_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();

            var isUM = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenance_IPIsAllowedToAccess_ExecuteRestClient()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            _maintenanceService.IsUnderMaintenance(siteId);

            _restClient.Received().Execute(Arg.Any<IRestRequest>());
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsValid_ReturnFalse()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "true";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            var isUM = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsFalse(isUM);
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsNotValid_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "false";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            var isUM = _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsValid_ReturnSetByPassUMToSession()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "true";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            _maintenanceService.IsUnderMaintenance(siteId);

            Assert.IsTrue(_maintenanceService.ByPassUM);
        }

        #endregion IsUnderMaintenance Test cases

        #region IsUnderMaintenanceOnRecheckIn Test Cases

        [Test]
        public void IsUnderMaintenanceOnRecheckIn_IsNotUMTime_ReturnFalse()
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime(false);

            _maintenanceService.IsUnderMaintenanceOnRecheckIn(siteId);

            _underMaintenanceController.Received().GetUnderMaintenanceTime(
                Arg.Is<Sites>(actualSiteId => actualSiteId == siteId));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsUnderMaintenanceOnRecheckIn_IsUMTime_ReturnByPassUM(bool isByPassUM)
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime();

            HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = isByPassUM;

            var isUM = _maintenanceService.IsUnderMaintenanceOnRecheckIn(siteId);

            Assert.AreEqual(isByPassUM, !isUM);
        }

        #endregion IsUnderMaintenanceOnRecheckIn Test Cases
    }
}