namespace ONEbook.UM.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.SessionState;
    using Fanex.Logging;
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
        private IList<Tuple<string, string>> _allowIPs;
        private string _clientIP;
        private ILogger _logger;
        private Sites _siteId;

        [SetUp]
        public void SetUp()
        {
            _underMaintenanceControlAdapter = Substitute.For<IUMControlAdapter>();
            _restClient = Substitute.For<IRestClient>();
            _logger = Substitute.For<ILogger>();
            _allowIPs = new List<Tuple<string, string>>();
            _clientIP = "10.18.200.209";
            _siteId = Sites.AccountSite;
            _maintenanceService = new MaintenanceService(_underMaintenanceControlAdapter, _restClient, _allowIPs, _clientIP, _logger, _siteId);
        }

        #region IsUnderMaintenanceOnLogin Test cases

        [Test]
        public void IsUnderMaintenanceOnLogin_SiteIdIsNotValid_ThrowArgumentOutOfRangeException()
        {
            _maintenanceService = new MaintenanceService(_underMaintenanceControlAdapter, _restClient, _allowIPs, _clientIP, _logger, (Sites)500);

            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(
                () => _maintenanceService.IsUnderMaintenanceOnLogin()));
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_ExecuteGetDowntime()
        {
            AssumeUMTime(isUMTime: false);

            _maintenanceService.IsUnderMaintenanceOnLogin();

            _underMaintenanceControlAdapter.Received().GetUnderMaintenanceTime(
                Arg.Is<Sites>(actualSiteId => actualSiteId == _siteId));
        }

        private void AssumeUMTime(bool isUMTime = true)
        {
            _underMaintenanceControlAdapter.GetUnderMaintenanceTime(Arg.Any<Sites>()).Returns(new UnderMaintenanceTime(isUMTime, string.Empty, string.Empty));
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_NowIsNotUMTime_ReturnFalse()
        {
            AssumeUMTime(isUMTime: false);

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin();

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
            FakeHttpContext(string.Empty);
            AssumeUMTime();

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin();

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_IPIsNotAllowedToAccess_ReturnTrue()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin();

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_IPIsAllowedToAccess_ExecuteRestClient()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            _maintenanceService.IsUnderMaintenanceOnLogin();

            _restClient.Received().Execute(Arg.Any<IRestRequest>());
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_SecretTokenIsValid_ReturnFalse()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "true";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin();

            Assert.IsFalse(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_SecretTokenIsNotValid_ReturnTrue()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "false";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            var isUM = _maintenanceService.IsUnderMaintenanceOnLogin();

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_SecretTokenIsValid_SetByPassUMToSession()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "true";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            _maintenanceService.IsUnderMaintenanceOnLogin();

            Assert.IsTrue(_maintenanceService.ByPassUM);
        }

        [Test]
        public void IsUnderMaintenanceOnLogin_TokenAPIThrowException_ReturnTrue()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(x => { throw new Exception(); });

            var isUMActual = _maintenanceService.IsUnderMaintenanceOnLogin();

            Assert.IsTrue(isUMActual);
        }

        #endregion IsUnderMaintenanceOnLogin Test cases

        #region IsUnderMaintenanceOnRecheckIn Test Cases

        [Test]
        public void IsUnderMaintenanceOnRecheckIn_SiteIdIsNotValid_ThrowArgumentOutOfRangeException()
        {
            _maintenanceService = new MaintenanceService(_underMaintenanceControlAdapter, _restClient, _allowIPs, _clientIP, _logger, (Sites)500);

            Assert.Throws<ArgumentOutOfRangeException>(new TestDelegate(
                () => _maintenanceService.IsUnderMaintenanceOnRecheckIn()));
        }

        [Test]
        public void IsUnderMaintenanceOnRecheckIn_NowIsNotUMTime_ReturnFalse()
        {
            AssumeUMTime(false);

            _maintenanceService.IsUnderMaintenanceOnRecheckIn();

            _underMaintenanceControlAdapter.Received().GetUnderMaintenanceTime(
                Arg.Is<Sites>(actualSiteId => actualSiteId == _siteId));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsUnderMaintenanceOnRecheckIn_NowIsUMTime_ReturnByPassUM(bool isByPassUM)
        {
            AssumeUMTime();

            HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = isByPassUM;

            var isUM = _maintenanceService.IsUnderMaintenanceOnRecheckIn();

            Assert.AreEqual(isByPassUM, !isUM);
        }

        [Test]
        public void IsUnderMaintenanceOnRecheckIn_ByPassUMInSessionIsNull_ReturnTrue()
        {
            AssumeUMTime();

            HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = null;

            var isUM = _maintenanceService.IsUnderMaintenanceOnRecheckIn();

            Assert.IsTrue(isUM);
        }

        #endregion IsUnderMaintenanceOnRecheckIn Test Cases
    }
}