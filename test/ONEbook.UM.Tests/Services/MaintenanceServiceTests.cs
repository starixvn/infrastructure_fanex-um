namespace ONEbook.UM.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.SessionState;
    using Fanex.Caching;
    using NSubstitute;
    using NUnit.Framework;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using RestSharp;

    [TestFixture]
    public class MaintenanceServiceTests
    {
        private MaintenanceService _maintenanceService;
        private IUMControlAdapter _underMaintenanceControlAdapter;
        private UM.Helpers.IRestClient restClient;
        private string clientIP;
        private ILoggingService loggingService;
        private IHttpContextHelper httpContextHelper;
        private IUMClientConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            _underMaintenanceControlAdapter = Substitute.For<IUMControlAdapter>();
            restClient = Substitute.For<UM.Helpers.IRestClient>();
            loggingService = Substitute.For<ILoggingService>();
            httpContextHelper = Substitute.For<IHttpContextHelper>();
            configuration = Substitute.For<IUMClientConfiguration>();
            clientIP = "10.18.200.209";

            _maintenanceService = new MaintenanceService(
                _underMaintenanceControlAdapter,
                restClient,
                clientIP,
                loggingService,
                httpContextHelper,
                configuration);
        }

        #region IsUnderMaintenance Test cases

        [Test]
        public async Task IsUnderMaintenance_ExecuteGetDowntime()
        {
            AssumeUMTime(isUMTime: false);

            await _maintenanceService.IsUnderMaintenance();

            await _underMaintenanceControlAdapter.Received().GetUnderMaintenanceTime();
        }

        private void AssumeUMTime(bool isUMTime = true)
        {
            _underMaintenanceControlAdapter.GetUnderMaintenanceTime().Returns(new UnderMaintenanceTime(isUMTime, string.Empty, string.Empty));
        }

        [Test]
        public async Task IsUnderMaintenance_NowIsNotUMTime_ReturnFalse()
        {
            AssumeUMTime(isUMTime: false);

            var isUM = await _maintenanceService.IsUnderMaintenance();

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

            restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

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

            restClient.Received().Execute(Arg.Any<IRestRequest>());
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

            restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

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

            restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

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

            restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

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

            restClient.Execute(Arg.Any<IRestRequest>()).Returns(x => { throw new Exception(); });

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

            Assert.IsFalse(isUnderMaintenance);
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