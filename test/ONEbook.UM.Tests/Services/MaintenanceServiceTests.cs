namespace ONEbook.UM.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using Xunit;

    public class MaintenanceServiceTests
    {
        private readonly IUMControlAdapter umControlAdapter;
        private readonly IRestClient restClient;
        private readonly ILoggingService loggingService;
        private readonly IHttpContextHelper httpContextHelper;
        private readonly IUMClientConfiguration configuration;
        private readonly IMaintenanceService maintenanceService;

        public MaintenanceServiceTests()
        {
            umControlAdapter = Substitute.For<IUMControlAdapter>();
            restClient = Substitute.For<IRestClient>();
            loggingService = Substitute.For<ILoggingService>();
            httpContextHelper = Substitute.For<IHttpContextHelper>();
            configuration = Substitute.For<IUMClientConfiguration>();
            maintenanceService = new MaintenanceService(
                umControlAdapter, restClient, "10.10.10.10", loggingService, httpContextHelper, configuration);
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            umControlAdapter.GetUnderMaintenanceTime().Returns(expectedUnderMaintenanceTime);

            // Act
            var result = await maintenanceService.GetUnderMaintenanceTime();

            // Assert
            Assert.Equal(expectedUnderMaintenanceTime, result);
        }

        [Fact]
        public async Task IsUnderMaintenance_NowIsNotUnderMaintenance_ReturnFalse()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(false, "3:00", "4:00");
            umControlAdapter.GetUnderMaintenanceTime().Returns(expectedUnderMaintenanceTime);

            // Act
            var result = await maintenanceService.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsUnderMaintenance_ByPassUM_ReturnFalse()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            httpContextHelper.GetSession("ONEbook.UM.ByPassUM").Returns("1");
            umControlAdapter.GetUnderMaintenanceTime().Returns(expectedUnderMaintenanceTime);

            // Act
            var result = await maintenanceService.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsUnderMaintenance_IsValidToken_ReturnFalse()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            const string token = "1234556";
            configuration.SiteId.Returns(1);
            umControlAdapter.GetUnderMaintenanceTime().Returns(expectedUnderMaintenanceTime);
            httpContextHelper.GetValueFromQueryString("UMToken").Returns(token);
            restClient.ExecuteGetAsync<bool>("/token/validate?SiteId=1&Token=1234556", null).Returns(true);

            // Act
            var result = await maintenanceService.IsUnderMaintenance();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsUnderMaintenance_IsValidToken_ThrowException_WriteErrorLog()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            const string token = "1234556";
            configuration.SiteId.Returns(1);
            umControlAdapter.GetUnderMaintenanceTime().Returns(expectedUnderMaintenanceTime);
            httpContextHelper.GetValueFromQueryString("UMToken").Returns(token);
            var exception = new Exception("Exception");
            restClient.ExecuteGetAsync<bool>("/token/validate", Arg.Any<object>()).Throws(exception);

            // Act
            await maintenanceService.IsUnderMaintenance();

            // Assert
            loggingService.Error(Arg.Is("Exception"), exception);
        }

        [Fact]
        public async Task GetAllUnderMaintenanceTime_Always_CallRestClient()
        {
            // Arrange
            var underMaintenanceTime = new Dictionary<int, UnderMaintenanceTime> { { 1, new UnderMaintenanceTime() } };
            restClient
                .ExecuteGetAsync<Dictionary<int, UnderMaintenanceTime>>(Arg.Is("/UnderMaintenance/GetAll"))
                .Returns(underMaintenanceTime);

            // Act
            var actualUnderMaintenanceTime = await maintenanceService.GetAllUnderMaintenanceTime();

            // Assert
            Assert.Equal(underMaintenanceTime, actualUnderMaintenanceTime);
        }

        [Fact]
        public async Task GetScheduledUnderMaintenance_Always_CallRestClient()
        {
            // Arrange
            var underMaintenanceTime = new Dictionary<int, UnderMaintenanceTime> { { 1, new UnderMaintenanceTime() } };
            restClient
                .ExecuteGetAsync<Dictionary<int, UnderMaintenanceTime>>(Arg.Is("/UnderMaintenance/GetScheduledUnderMaintenance"))
                .Returns(underMaintenanceTime);

            // Act
            var actualUnderMaintenanceTime = await maintenanceService.GetScheduledUnderMaintenance();

            // Assert
            Assert.Equal(underMaintenanceTime, actualUnderMaintenanceTime);
        }
    }
}