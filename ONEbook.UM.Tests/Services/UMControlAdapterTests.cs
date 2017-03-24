namespace ONEbook.UM.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Configurations;
    using NSubstitute;
    using NUnit.Framework;
    using RestSharp;
    using UM.Services;
    using UM.Services.Impl;

    public class UMControlAdapterTests
    {
        private IRestClient _restClient;
        private ILoggingService _loggingService;
        private int _siteId;
        private ICachingService _cachingService;
        private UMControlAdapter _underMaintenanceControlAdapter;

        [SetUp]
        public void SetUp()
        {
            _restClient = Substitute.For<IRestClient>();
            _loggingService = Substitute.For<ILoggingService>();
            _cachingService = Substitute.For<ICachingService>();
            _siteId = 2000000;

            UMClientManager.WhenError((message) => { });
            _underMaintenanceControlAdapter = new UMControlAdapter(_siteId, _cachingService, _restClient, _loggingService);
        }

        [Test]
        public void GetUnderMaintenance_Always_ExecuteRestClient()
        {
            // Arrange
            IRestResponse restResponse = Substitute.For<IRestResponse>();
            restResponse.StatusCode.Returns(HttpStatusCode.OK);
            restResponse.Content.Returns("{\"From\": null, \"To\": null, \"IsUnderMaintenanceTime\": true, \"ConnectionResult\": { \"IsOk\": true, \"Message\": null } }");

            _underMaintenanceControlAdapter.GetUnderMaintenance();

            _restClient.Received().Execute(Arg.Is<IRestRequest>(
                request =>
                    request.Resource == "/UnderMaintenance/get"
                    && request.Method == Method.GET));
        }

        [Test]
        public void GetUnderMaintenance_ConnectToUMServiceOK_ReturnUnderMaintenanceInfo()
        {
            // Arrange
            IRestResponse restResponse = Substitute.For<IRestResponse>();
            restResponse.StatusCode.Returns(HttpStatusCode.OK);
            restResponse.Content.Returns("{\"From\": null, \"To\": null, \"IsUnderMaintenanceTime\": true, \"ConnectionResult\": { \"IsOk\": true, \"Message\": null } }");

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);

            // Act
            var underMaintenanceInfo = _underMaintenanceControlAdapter.GetUnderMaintenance();

            // Assert
            Assert.IsTrue(
                underMaintenanceInfo.IsUnderMaintenanceTime
                && underMaintenanceInfo.ConnectionResult.IsOk);
        }

        [Test]
        public void GetUnderMaintenance_ConnectToUMServiceFailed_ReturnUnderMaintenanceWithFailedMessage()
        {
            // Arrange
            IRestResponse restResponse = Substitute.For<IRestResponse>();
            restResponse.StatusCode.Returns(HttpStatusCode.NotFound);
            restResponse.Content.Returns("Failed Message");

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);
            _restClient.BaseUrl.Returns(new Uri("http://demo.uri"));
            UMClientConfiguration.ServiceUriList = new List<string>
            {
                "http://demo.uri"
            };

            // Act
            var underMaintenanceInfo = _underMaintenanceControlAdapter.GetUnderMaintenance();

            // Assert
            Assert.IsTrue(
                !underMaintenanceInfo.IsUnderMaintenanceTime
                && !underMaintenanceInfo.ConnectionResult.IsOk
                && underMaintenanceInfo.ConnectionResult.Message
                == "Base Url: http://demo.uri/\r\nStatus Code: 404\r\nResponse Content: Failed Message\r\nResponse Error Message: ");
        }

        [Test]
        public void GetUnderMaintenance_ConnectToUMServiceFailed_LogErrorMessage()
        {
            // Arrange
            IRestResponse restResponse = Substitute.For<IRestResponse>();
            restResponse.StatusCode.Returns(HttpStatusCode.NotFound);
            restResponse.Content.Returns("Failed Message");

            _restClient.Execute(Arg.Any<IRestRequest>()).Returns(restResponse);
            _restClient.BaseUrl.Returns(new Uri("http://demo.uri"));

            // Act
            _underMaintenanceControlAdapter.GetUnderMaintenance();

            // Assert
            _loggingService.Received().Error("Base Url: http://demo.uri/\r\nStatus Code: 404\r\nResponse Content: Failed Message\r\nResponse Error Message: ");
        }

        [Test]
        public void GetUnderMaintenance_ThrowException_LogErrorMessage()
        {
            // Arrange
            var fakeException = new Exception("Fake Exception");
            _restClient
                .When(client => client.Execute(Arg.Any<IRestRequest>()))
                .Do(call =>
                {
                    throw fakeException;
                });

            // Act
            _underMaintenanceControlAdapter.GetUnderMaintenance();

            // Assert
            _loggingService.Received().Error("Fake Exception", fakeException);
        }

        [Test]
        public void GetUnderMaintenance_ThrowException_ReturnUnderMaintenanceWithFailedMessage()
        {
            // Arrange
            var fakeException = new Exception("Fake Exception");
            _restClient
                .When(client => client.Execute(Arg.Any<IRestRequest>()))
                .Do(call =>
                {
                    throw fakeException;
                });

            // Act
            var underMaintenanceInfo = _underMaintenanceControlAdapter.GetUnderMaintenance();

            // Assert
            Assert.IsTrue(
                !underMaintenanceInfo.IsUnderMaintenanceTime
                && !underMaintenanceInfo.ConnectionResult.IsOk
                && underMaintenanceInfo.ConnectionResult.Message == "Fake Exception");
        }
    }
}