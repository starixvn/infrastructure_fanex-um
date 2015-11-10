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
        private IUMControlAdapter _underMaintenanceControlAdapter;
        private IRestClient _restClient;
        private IList<string> _allowIPs;
        private string _clientIP;

        [SetUp]
        public void SetUp()
        {
            _underMaintenanceControlAdapter = Substitute.For<IUMControlAdapter>();
            _restClient = Substitute.For<IRestClient>();
            _allowIPs = new List<string>();
            _clientIP = "10.18.200.209";
            _maintenanceService = new MaintenanceService(_underMaintenanceControlAdapter, _restClient, _allowIPs, _clientIP);
        }

        #region IsUnderMaintenanceOnLogin Test cases

        [Test]
        public void IsUnderMaintenanceOnLogin_ExecuteGetDowntime()
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime(isUMTime: false);

            _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

            _underMaintenanceControlAdapter.Received().GetUnderMaintenanceTime(
                Arg.Is<Sites>(actualSiteId => actualSiteId == siteId));
        }

        private void AssumeUMTime(bool isUMTime = true)
        {
            _underMaintenanceControlAdapter.GetUnderMaintenanceTime(Arg.Any<Sites>()).Returns(new UnderMaintenanceTime
            {
                IsUnderMaintenanceTime = isUMTime
            });
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_NowIsNotUMTime_ReturnFalse()
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime(isUMTime: false);

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

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
        public void IsUnderMaintenanceOnLogin_RequestDoesNotContainSecretToken_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            FakeHttpContext(string.Empty);
            AssumeUMTime();

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_IPIsNotAllowedToAccess_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_IPIsAllowedToAccess_ExecuteRestClient()
        {
            var siteId = Sites.AccountSite;
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add("10.18.200.209");

            _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

            _restClient.Received().Execute(Arg.Any<IRestRequest>());
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_SecretTokenIsValid_ReturnFalse()
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

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

            Assert.IsFalse(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_SecretTokenIsNotValid_ReturnTrue()
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

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_SecretTokenIsValid_SetByPassUMToSession()
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

            _maintenanceService.IsUnderMaintenanceOnLogin(siteId);

            Assert.IsTrue(_maintenanceService.ByPassUM);
        }

        #endregion IsUnderMaintenance Test cases

        #region IsUnderMaintenanceOnRecheckIn Test Cases

        [Test]
        public void IsUnderMaintenanceOnRecheckIn_NowIsNotUMTime_ReturnFalse()
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime(false);

            _maintenanceService.IsUnderMaintenanceOnRecheckIn(siteId);

            _underMaintenanceControlAdapter.Received().GetUnderMaintenanceTime(
                Arg.Is<Sites>(actualSiteId => actualSiteId == siteId));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsUnderMaintenanceOnRecheckIn_NowIsUMTime_ReturnByPassUM(bool isByPassUM)
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime();

            HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = isByPassUM;

            var isUM = _maintenanceService.IsUnderMaintenanceOnRecheckIn(siteId);

            Assert.AreEqual(isByPassUM, !isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnRecheckIn_ByPassUMInSessionIsNull_ReturnTrue()
        {
            var siteId = Sites.AccountSite;
            AssumeUMTime();

            HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = null;

            var isUM = _maintenanceService.IsUnderMaintenanceOnRecheckIn(siteId);

            Assert.IsTrue(isUM);
        }

        #endregion IsUnderMaintenanceOnRecheckIn Test Cases
    }
}