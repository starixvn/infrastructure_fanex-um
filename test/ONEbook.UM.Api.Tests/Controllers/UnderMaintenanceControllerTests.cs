namespace ONEbook.UM.Api.Tests.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Services;
using Models;
using NSubstitute;
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
    public async Task Get_Always_ReturnUnderMaintenanceTime()
    {
        // Arrange
        var underMaintenance = new UnderMaintenanceTime(true, "3:00", "4:00");
        underMaintenanceControlAdapter.GetUnderMaintenanceTime(Arg.Is(1)).Returns(Task.FromResult(underMaintenance));

        // Act
        var actualUnderMaintenance = await controller.Get(1);

        // Assert
        Assert.Equal(underMaintenance, actualUnderMaintenance);
    }

    [Fact]
    public async Task GetAll_Always_ReturnUnderMaintenanceTimes()
    {
        // Arrange
        var underMaintenances = new Dictionary<int, UnderMaintenanceTime> { { 1, new UnderMaintenanceTime(true, "3:00", "4:00") } };
        underMaintenanceControlAdapter.GetAllUnderMaintenanceTime().Returns(underMaintenances);

        // Act
        var actualUnderMaintenances = await controller.GetAll();

        // Assert
        Assert.Equal(underMaintenances, actualUnderMaintenances);
    }

    [Fact]
    public async Task GetScheduledUnderMaintenance_Always_ReturnUnderMaintenanceTimes()
    {
        // Arrange
        var underMaintenances = new Dictionary<int, UnderMaintenanceTime> { { 1, new UnderMaintenanceTime(true, "3:00", "4:00") } };
        underMaintenanceControlAdapter.GetScheduledUnderMaintenanceInformation().Returns(underMaintenances);

        // Act
        var actualUnderMaintenances = await controller.GetScheduledUnderMaintenance();

        // Assert
        Assert.Equal(underMaintenances, actualUnderMaintenances);
    }
}