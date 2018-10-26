namespace ONEbook.UM.Api.Tests.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using ONEbook.UM.Api.Configurations;
    using ONEbook.UM.Api.Services;
    using Xunit;

    public class UMControlAdapterTests
    {
        private static readonly IDictionary<int, int> OldNewSiteIdMappers = new Dictionary<int, int>
        {
            { 8, 3000000 },
            { 2000000, 2000000 },
            { 2000001, 2000001 },
            { 2000002, 2000002 },
            { 2000003, 2000003 },
            { 2000005, 2000005 },
            { 2000007, 2000007 },
            { 4100100, 4100300 },
        };

        private readonly IUMControlAdapter umControlAdapter;

        public UMControlAdapterTests()
        {
            umControlAdapter = new UMControlAdapter();
            ConfigurationManager.AppSettings["EnableFakeUM"] = "false";
            ConfigurationManager.AppSettings["MinutesCheckUM"] = "0,5,10,15,20,25,30,35,40,45,50,55";
        }

        [Fact]
        public void GetUnderMaintenanceTime_HasFakeUM_ReturnFakeUM()
        {
            // Arrange
            ConfigurationManager.AppSettings["EnableFakeUM"] = "true";

            // Act
            var actualUnderMaintenance = umControlAdapter.GetUnderMaintenanceTime(1);

            // Assert
            Assert.True(actualUnderMaintenance.IsUnderMaintenanceTime);
        }

        [Fact]
        public void GetUnderMaintenanceTime_EnableUnscheduleUM_ReturnUnscheduleUM()
        {
            // Arrange
            UnderMaintenanceApiConfiguration.Settings = new UnderMaintenanceApiConfiguration
            {
                EnableUnscheduledUM = true,
                UMFrom = "3:00",
                UMTo = "4:00"
            };

            // Act
            var actualUnderMaintenance = umControlAdapter.GetUnderMaintenanceTime(1);

            // Assert
            Assert.True(actualUnderMaintenance.IsUnderMaintenanceTime);
            Assert.Equal("3:00", actualUnderMaintenance.From);
            Assert.Equal("4:00", actualUnderMaintenance.To);
        }

        [Fact]
        public void GetUnderMaintenanceTime_EnableUM_ReturnUM()
        {
            // Arrange
            UnderMaintenanceApiConfiguration.Settings = new UnderMaintenanceApiConfiguration
            {
                EnableCheckUM = true
            };

            // Act
            var actualUnderMaintenance = umControlAdapter.GetUnderMaintenanceTime(1);

            // Assert
            Assert.False(actualUnderMaintenance.IsUnderMaintenanceTime);
            Assert.Equal("0:00", actualUnderMaintenance.From);
            Assert.Equal("0:00", actualUnderMaintenance.To);
        }

        [Fact]
        public void GetAllUnderMaintenanceTime_Always_ReturnAllUM()
        {
            // Arrange
            UnderMaintenanceApiConfiguration.Settings = new UnderMaintenanceApiConfiguration
            {
                EnableCheckUM = true
            };

            // Act
            var actualUnderMaintenances = umControlAdapter.GetAllUnderMaintenanceTime();

            // Assert
            Assert.True(OldNewSiteIdMappers.All(kv => actualUnderMaintenances.ContainsKey(kv.Key)));
        }

        [Fact]
        public void GetScheduledUnderMaintenanceInformation_HasFakeUMInfo_ReturnFakeUM()
        {
            // Arrange
            ConfigurationManager.AppSettings["EnableFakeUM"] = "true";

            // Act
            var actualUnderMaintenances = umControlAdapter.GetScheduledUnderMaintenanceInformation();

            // Assert
            Assert.True(OldNewSiteIdMappers.All(kv => actualUnderMaintenances.ContainsKey(kv.Key)));
            Assert.True(actualUnderMaintenances.All(um => um.Value.IsUnderMaintenanceTime));
        }

        [Fact]
        public void GetScheduledUnderMaintenanceInformation_EnableUnscheduleUM_ReturnUnscheduledUM()
        {
            // Arrange
            UnderMaintenanceApiConfiguration.Settings = new UnderMaintenanceApiConfiguration
            {
                EnableUnscheduledUM = true,
                UMFrom = "3:00",
                UMTo = "4:00"
            };

            // Act
            var actualUnderMaintenances = umControlAdapter.GetScheduledUnderMaintenanceInformation();

            // Assert
            Assert.True(OldNewSiteIdMappers.All(kv => actualUnderMaintenances.ContainsKey(kv.Key)));
            Assert.True(actualUnderMaintenances.All(um => um.Value.IsUnderMaintenanceTime));
            Assert.True(actualUnderMaintenances.All(um => um.Value.From == "3:00"));
            Assert.True(actualUnderMaintenances.All(um => um.Value.To == "4:00"));
        }

        [Fact]
        public void GetScheduledUnderMaintenanceInformation_EnableCheckUM_ReturnRealUM()
        {
            // Arrange
            UnderMaintenanceApiConfiguration.Settings = new UnderMaintenanceApiConfiguration
            {
                EnableCheckUM = true
            };

            // Act
            var actualUnderMaintenances = umControlAdapter.GetScheduledUnderMaintenanceInformation();

            // Assert
            Assert.True(OldNewSiteIdMappers.All(kv => actualUnderMaintenances.ContainsKey(kv.Key)));
            Assert.True(actualUnderMaintenances.All(um => !um.Value.IsUnderMaintenanceTime));
            Assert.True(actualUnderMaintenances.All(um => um.Value.From == "0:00"));
            Assert.True(actualUnderMaintenances.All(um => um.Value.To == "0:00"));
        }
    }
}