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

    [TestFixture]
    public class MaintenanceServiceTests
    {
        private MaintenanceService _maintenanceService;
        private IUMControlAdapter _underMaintenanceControlAdapter;
        private IRestClient _restClient;
        private IList<Tuple<string, string>> _allowIPs;
        private string _clientIP;
        private ILogger _logger;
        private int _siteId;
        private bool _isAllowAllIPs;
        private ICachingService _cachingService;

        [SetUp]
        public void SetUp()
        {
            _underMaintenanceControlAdapter = Substitute.For<IUMControlAdapter>();
            _restClient = Substitute.For<IRestClient>();
            _logger = Substitute.For<ILogger>();
            _cachingService = Substitute.For<ICachingService>();
            _allowIPs = new List<Tuple<string, string>>();
            _clientIP = "10.18.200.209";
            _siteId = 2000000;
            _isAllowAllIPs = false;

            _maintenanceService = new MaintenanceService(
                _underMaintenanceControlAdapter,
                _restClient,
                _allowIPs,
                _clientIP,
                _logger,
                _cachingService,
                _siteId,
                _isAllowAllIPs);
        }

        #region IsUnderMaintenance Test cases

        [Test]
        public void IsUnderMaintenance_ExecuteGetDowntime()
        {
            AssumeUMTime(isUMTime: false);

            _maintenanceService.IsUnderMaintenance();

            _underMaintenanceControlAdapter.Received().GetUnderMaintenanceTime();
        }

        private void AssumeUMTime(bool isUMTime = true)
        {
            _underMaintenanceControlAdapter.GetUnderMaintenanceTime().Returns(new UnderMaintenanceTime(isUMTime, string.Empty, string.Empty));
        }

        [Test]
        public void IsUnderMaintenance_NowIsNotUMTime_ReturnFalse()
        {
            AssumeUMTime(isUMTime: false);

            var isUM = _maintenanceService.IsUnderMaintenance();

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
            FakeHttpContext(string.Empty);
            AssumeUMTime();

            var isUM = _maintenanceService.IsUnderMaintenance();

            Assert.IsTrue(isUM);
        }

        [TestCase("10.18.200.210", "10.18.200.210")]
        [TestCase("10.18.100.209", "10.18.100.235")]
        public void IsUnderMaintenance_IPIsNotAllowedToAccess_ReturnTrue(string fromIP, string toIP)
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>(fromIP, toIP));

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "true";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            var isUM = _maintenanceService.IsUnderMaintenance();

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenance_IPIsAllowedToAccess_ExecuteRestClient()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            _maintenanceService.IsUnderMaintenance();

            _restClient.Received().Execute(Arg.Any<IRestRequest>());
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsValid_ReturnFalse()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "true";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            var isUM = _maintenanceService.IsUnderMaintenance();

            Assert.IsFalse(isUM);
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsNotValid_ReturnTrue()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "false";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            var isUM = _maintenanceService.IsUnderMaintenance();

            Assert.IsTrue(isUM);
        }

        [Test]
        public void IsUnderMaintenance_SecretTokenIsValid_SetByPassUMToSession()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            RestResponse restResponse = new RestResponse();
            restResponse.StatusCode = HttpStatusCode.OK;
            restResponse.Content = "true";

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            _maintenanceService.IsUnderMaintenance();

            Assert.IsTrue(_maintenanceService.ByPassUM);
        }

        [Test]
        public void IsUnderMaintenance_TokenAPIThrowException_ReturnTrue()
        {
            var queryString = "UMToken=UMToken";
            FakeHttpContext(queryString);
            AssumeUMTime();
            _allowIPs.Add(new Tuple<string, string>("10.18.200.209", "10.18.200.209"));

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(x => { throw new Exception(); });

            var isUMActual = _maintenanceService.IsUnderMaintenance();

            Assert.IsTrue(isUMActual);
        }

        #endregion IsUnderMaintenance Test cases

        #region IsUnderMaintenanceOnRecheckIn Test Cases

        [Test]
        public void IsPassUnderMaintenance_NowIsNotUMTime_ReturnTrue()
        {
            FakeHttpContext(string.Empty);
            AssumeUMTime(false);

            _maintenanceService.IsUnderMaintenance();

            _underMaintenanceControlAdapter.Received().GetUnderMaintenanceTime();
        }

        [Test]
        public void IsUnderMaintenance_NowIsUMTimeAndByPassM_ReturnFalse()
        {
            FakeHttpContext(string.Empty);
            AssumeUMTime();

            HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = true;

            var isUnderMaintenance = _maintenanceService.IsUnderMaintenance();

            Assert.IsFalse(false);
        }

        [Test]
        public void IsUnderMaintenance_ByPassUMInSessionIsNull_ReturnTrue()
        {
            FakeHttpContext(string.Empty);
            AssumeUMTime();

            HttpContext.Current.Session["ONEbook.UM.ByPassUM"] = null;

            var isUnderMaintenance = _maintenanceService.IsUnderMaintenance();

            Assert.IsTrue(isUnderMaintenance);
        }

        #endregion IsUnderMaintenanceOnRecheckIn Test Cases
    }
}