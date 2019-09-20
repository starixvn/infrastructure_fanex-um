using System;
using KellermanSoftware.CompareNetObjects;
using NSubstitute.ExceptionExtensions;

namespace ONEbook.UM.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Configurations;
    using Models;
    using NSubstitute;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using Xunit;

    public class MaintenanceServiceTests
    {
        private readonly IUMRestClient restClient;
        private readonly IHttpContextHelper httpContextHelper;
        private readonly IUMCacheService cacheService;
        private readonly UMClientConfiguration configuration;
        private readonly IMaintenanceService maintenanceService;
        private readonly CompareLogic comparer;

        public MaintenanceServiceTests()
        {
            comparer = new CompareLogic();
            restClient = Substitute.For<IUMRestClient>();
            httpContextHelper = Substitute.For<IHttpContextHelper>();
            cacheService = Substitute.For<IUMCacheService>();
            configuration = new UMClientConfiguration { SiteId = 1 };
            maintenanceService = new MaintenanceService(
                restClient, cacheService, httpContextHelper, configuration);
        }

        [Fact]
        public async Task GetUnderMaintenanceTimeAsync_Always_GetExpectedUMInfo()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            restClient.ExecuteGetAsync<UnderMaintenanceTime>($"/UnderMaintenance/get?siteId=1")
                .Returns(expectedUnderMaintenanceTime);

            // Act
            var result = await maintenanceService.GetUnderMaintenanceTimeAsync();

            // Assert
            Assert.Equal(expectedUnderMaintenanceTime, result);
        }

        [Fact]
        public async Task IsUnderMaintenance_NowIsNotUnderMaintenance_ReturnFalse()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(false, "3:00", "4:00");
            restClient.ExecuteGetAsync<UnderMaintenanceTime>($"/UnderMaintenance/get?siteId=1")
                .Returns(expectedUnderMaintenanceTime);

            // Act
            var result = await maintenanceService.IsUnderMaintenanceAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsUnderMaintenance_ByPassUM_ReturnFalse()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            httpContextHelper.GetSession("ONEbook.UM.ByPassUM").Returns("1");
            restClient.ExecuteGetAsync<UnderMaintenanceTime>($"/UnderMaintenance/get?siteId=1")
                .Returns(expectedUnderMaintenanceTime);

            // Act
            var result = await maintenanceService.IsUnderMaintenanceAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsUnderMaintenance_IsValidToken_ReturnFalse()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            const string token = "1234556";
            restClient.ExecuteGetAsync<UnderMaintenanceTime>($"/UnderMaintenance/get?siteId=1")
                .Returns(expectedUnderMaintenanceTime);
            httpContextHelper.GetValueFromQueryString("UMToken").Returns(token);
            restClient.ExecuteGetAsync<bool>("/token/validate?SiteId=1&Token=1234556").Returns(true);

            // Act
            var result = await maintenanceService.IsUnderMaintenanceAsync();

            // Assert
            Assert.False(result);
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
            var actualUnderMaintenanceTime = await maintenanceService.GetAllUnderMaintenanceTimeAsync();

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
            var actualUnderMaintenanceTime = await maintenanceService.GetScheduledUnderMaintenanceAsync();

            // Assert
            Assert.Equal(underMaintenanceTime, actualUnderMaintenanceTime);
        }

        [Fact]
        public async Task GetUnderMaintenanceTimeAsync_EnableUnscheduleUM_ReturnExpectedUnscheduleUMInfo()
        {
            // Arrange
            configuration.EnableUnscheduledUM = true;
            configuration.UMFrom = "3:00";
            configuration.UMTo = "4:00";

            // Act
            var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

            // Assert

            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }

        [Fact]
        public async Task GetUnderMaintenanceTimeAsync_EnableCheckUM_CacheHasUMInfo_ReturnDataFromCache()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            configuration.EnableCheckUM = true;
            configuration.SiteId = 1;
            cacheService.Get<UnderMaintenanceTime>("ONEbook.UM.Site.1").Returns(expectedUnderMaintenanceTime);

            // Act
            var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_EnableCheckUM_CacheHasNoUMInfo_GetAndReturnDataFromAPI()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            configuration.EnableCheckUM = true;
            configuration.ResultCacheDuration = 60;
            restClient
                .ExecuteGetAsync<UnderMaintenanceTime>("/UnderMaintenance/get?siteId=1")
                .Returns(expectedUnderMaintenanceTime);

            // Act
            var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
            cacheService.Received().Set("ONEbook.UM.Site.1",
                Arg.Is(expectedUnderMaintenanceTime),
                TimeSpan.FromSeconds(60));
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_EnableCheckUM_GetDataThrowException_ReturnFailUMInfo()
        {
            // Arrange
            var expectedUnderMaintenanceTime = UnderMaintenanceTime.CreateFailedConnection("Error");
            configuration.EnableCheckUM = true;
            configuration.ResultCacheDuration = 60;
            var exception = new Exception("Error");
            restClient
                .ExecuteGetAsync<UnderMaintenanceTime>("/UnderMaintenance/get?siteId=1")
                .Throws(exception);

            // Act
            var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_NotEnableCheckUM_ReturnNotUMInfo()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(false, "0:00", "0:00");
            configuration.EnableCheckUM = false;

            // Act
            var underMaintenanceTime = await maintenanceService.GetUnderMaintenanceTimeAsync();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }
    }
}