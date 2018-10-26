namespace ONEbook.UM.Api.Tests.Controllers
{
    using System.Collections.Generic;
    using NSubstitute;
    using ONEbook.UM.Api.Controllers;
    using ONEbook.UM.Api.Models;
    using ONEbook.UM.Api.Services;
    using Xunit;

    public class UnderMaintenanceControllerTests
    {
        private readonly IUMControlAdapter underMaintenanceControlAdapter;
        private readonly UnderMaintenanceController controller;

        public UnderMaintenanceControllerTests()
        {
            underMaintenanceControlAdapter = Substitute.For<IUMControlAdapter>();
            controller = new UnderMaintenanceController(underMaintenanceControlAdapter);
        }

        [Fact]
        public void Get_Always_ReturnUnderMaintenanceTime()
        {
            // Arrange
            var underMaintenance = new UnderMaintenanceTime(true, "3:00", "4:00");
            underMaintenanceControlAdapter.GetUnderMaintenanceTime(Arg.Is(1)).Returns(underMaintenance);

            // Act
            var actualUnderMaintenance = controller.Get(1);

            // Assert
            Assert.Equal(underMaintenance, actualUnderMaintenance);
        }

        [Fact]
        public void GetAll_Always_ReturnUnderMaintenanceTimes()
        {
            // Arrange
            var underMaintenances = new Dictionary<int, UnderMaintenanceTime> { { 1, new UnderMaintenanceTime(true, "3:00", "4:00") } };
            underMaintenanceControlAdapter.GetAllUnderMaintenanceTime().Returns(underMaintenances);

            // Act
            var actualUnderMaintenances = controller.GetAll();

            // Assert
            Assert.Equal(underMaintenances, actualUnderMaintenances);
        }

        [Fact]
        public void GetScheduledUnderMaintenance_Always_ReturnUnderMaintenanceTimes()
        {
            // Arrange
            var underMaintenances = new Dictionary<int, UnderMaintenanceTime> { { 1, new UnderMaintenanceTime(true, "3:00", "4:00") } };
            underMaintenanceControlAdapter.GetScheduledUnderMaintenanceInformation().Returns(underMaintenances);

            // Act
            var actualUnderMaintenances = controller.GetScheduledUnderMaintenance();

            // Assert
            Assert.Equal(underMaintenances, actualUnderMaintenances);
        }
    }
}