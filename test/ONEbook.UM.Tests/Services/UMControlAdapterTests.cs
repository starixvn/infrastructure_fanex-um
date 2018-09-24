namespace ONEbook.UM.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using KellermanSoftware.CompareNetObjects;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using Xunit;

    public class UMControlAdapterTests
    {
        private readonly ICacheService cacheService;
        private readonly IRestClient restClient;
        private readonly IUMClientConfiguration configuration;
        private readonly IUMControlAdapter umControlAdapter;
        private readonly CompareLogic comparer;

        public UMControlAdapterTests()
        {
            cacheService = Substitute.For<ICacheService>();
            restClient = Substitute.For<IRestClient>();
            configuration = Substitute.For<IUMClientConfiguration>();
            umControlAdapter = new UMControlAdapter(
                cacheService,
                restClient,
                configuration);

            comparer = new CompareLogic();
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_EnableUnscheduleUM_ReturnExpectedUnscheduleUMInfo()
        {
            // Arrange
            configuration.EnableUnscheduledUM.Returns(true);
            configuration.UMFrom.Returns("3:00");
            configuration.UMTo.Returns("4:00");

            // Act
            var underMaintenanceTime = await umControlAdapter.GetUnderMaintenanceTime();

            // Assert

            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_EnableCheckUM_CacheHasUMInfo_ReturnDataFromCache()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            configuration.EnableCheckUM.Returns(true);
            configuration.SiteId.Returns(1);
            cacheService.Get<UnderMaintenanceTime>("ONEbook.UM.Site.1").Returns(expectedUnderMaintenanceTime);

            // Act
            var underMaintenanceTime = await umControlAdapter.GetUnderMaintenanceTime();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_EnableCheckUM_CacheHasNoUMInfo_GetAndReturnDataFromAPI()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(true, "3:00", "4:00");
            configuration.EnableCheckUM.Returns(true);
            configuration.SiteId.Returns(1);
            configuration.ResultCacheDuration.Returns(60);
            restClient
                .ExecuteGetAsync<UnderMaintenanceTime>("/UnderMaintenance/get?siteId=1")
                .Returns(expectedUnderMaintenanceTime);

            // Act
            var underMaintenanceTime = await umControlAdapter.GetUnderMaintenanceTime();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
            cacheService.Received().Set("ONEbook.UM.Site.1", Arg.Is(expectedUnderMaintenanceTime), Arg.Any<CacheItemOptions>());
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_EnableCheckUM_GetDataThrowException_ReturnFailUMInfo()
        {
            // Arrange
            var expectedUnderMaintenanceTime = UnderMaintenanceTime.CreateFailedConnection("Error");
            configuration.EnableCheckUM.Returns(true);
            configuration.SiteId.Returns(1);
            configuration.ResultCacheDuration.Returns(60);
            var exception = new Exception("Error");
            restClient
                .ExecuteGetAsync<UnderMaintenanceTime>("/UnderMaintenance/get?siteId=1")
                .Throws(exception);

            // Act
            var underMaintenanceTime = await umControlAdapter.GetUnderMaintenanceTime();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }

        [Fact]
        public async Task GetUnderMaintenanceTime_NotEnableCheckUM_ReturnNotUMInfo()
        {
            // Arrange
            var expectedUnderMaintenanceTime = new UnderMaintenanceTime(false, "0:00", "0:00");
            configuration.EnableCheckUM.Returns(false);

            // Act
            var underMaintenanceTime = await umControlAdapter.GetUnderMaintenanceTime();

            // Assert
            Assert.True(comparer.Compare(expectedUnderMaintenanceTime, underMaintenanceTime).AreEqual);
        }
    }
}