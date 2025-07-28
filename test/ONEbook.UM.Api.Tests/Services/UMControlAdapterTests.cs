namespace ONEbook.UM.Api.Tests.Services;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Services;
using Configurations;
using Constants;
using Microsoft.Extensions.Options;
using Models;
using NSubstitute;
using Xunit;

public class UMControlAdapterTests
{
    private static readonly IDictionary<int, int> OldNewSiteIdMappers = new Dictionary<int, int>
    {
        { (int)Sites.MBCAgencySite, (int)Sites.MBCAgencySite },
        { (int)Sites.IndoAgencySite, (int)Sites.IndoAgencySite },
        { (int)Sites._789yAgencySite, (int)Sites._789yAgencySite },
        { (int)Sites.Chatbot, (int)Sites.Chatbot },
        { (int)Sites._777AgencySite, (int)Sites._777AgencySite },
        { (int)Sites.AccountSite, (int)Sites.AccountSite },
        { (int)Sites.LRF, (int)Sites.LRF },
        { (int)Sites.OWA, (int)Sites.OWA },
        { (int)Sites.Aff12Bet, (int)Sites.Aff12Bet },
        { (int)Sites.RMS, (int)Sites.RMS },
        { (int)Sites.OddsFeed, (int)Sites.OddsFeed },
        { (int)Sites._789y, 4_100_300 },
        { (int)Sites.Test, (int)Sites.Test }
    };

    private readonly IOptions<UnderMaintenanceApiConfiguration> options = Substitute.For<IOptions<UnderMaintenanceApiConfiguration>>();
    private readonly IFakeUMControlAdapter fakeUMControlAdapter = Substitute.For<IFakeUMControlAdapter>();
    private readonly HttpClient httpClient = Substitute.For<HttpClient>();

    [Fact]
    public async Task GetUnderMaintenanceTime_HasFakeUM_ReturnFakeUM()
    {

        // Arrange
        fakeUMControlAdapter.GetUnderMaintenance().Returns(new UnderMaintenanceTime(true, "3:00", "4:00"));

        var umControlAdapter = new UMControlAdapter(options, fakeUMControlAdapter, httpClient);

        // Act
        var actualUnderMaintenance = await umControlAdapter.GetUnderMaintenanceTime(1);

        // Assert
        Assert.True(actualUnderMaintenance.IsUnderMaintenanceTime);
    }

    [Fact]
    public async Task GetUnderMaintenanceTime_EnableUnscheduleUM_ReturnUnscheduleUM()
    {
        // Arrange
        options.Value.Returns(new UnderMaintenanceApiConfiguration
        {
            EnableUnscheduledUM = true,
            UMFrom = "3:00",
            UMTo = "4:00"
        });

        var umControlAdapter = new UMControlAdapter(options, fakeUMControlAdapter, httpClient);

        // Act
        var actualUnderMaintenance = await umControlAdapter.GetUnderMaintenanceTime(1);

        // Assert
        Assert.True(actualUnderMaintenance.IsUnderMaintenanceTime);
        Assert.Equal("3:00", actualUnderMaintenance.From);
        Assert.Equal("4:00", actualUnderMaintenance.To);
    }

    [Fact]
    public async Task GetScheduledUnderMaintenanceInformation_EnableUnscheduleUM_ReturnUnscheduledUM()
    {
        // Arrange
        options.Value.Returns(new UnderMaintenanceApiConfiguration
        {
            EnableUnscheduledUM = true,
            UMFrom = "3:00",
            UMTo = "4:00"
        });

        var umControlAdapter = new UMControlAdapter(options, fakeUMControlAdapter, httpClient);
        // Act
        var actualUnderMaintenances = await umControlAdapter.GetScheduledUnderMaintenanceInformation();

        // Assert
        Assert.True(OldNewSiteIdMappers.All(kv => actualUnderMaintenances.ContainsKey(kv.Key)));
        Assert.True(actualUnderMaintenances.All(um => um.Value.IsUnderMaintenanceTime));
        Assert.True(actualUnderMaintenances.All(um => um.Value.From == "3:00"));
        Assert.True(actualUnderMaintenances.All(um => um.Value.To == "4:00"));
    }

    [Fact]
    public async Task GetScheduledUnderMaintenanceInformation_EnableCheckUM_ReturnRealUM()
    {
        // Arrange
        options.Value.Returns(new UnderMaintenanceApiConfiguration
        {
            EnableCheckUM = true
        });
        var umControlAdapter = new UMControlAdapter(options, fakeUMControlAdapter, httpClient);

        // Act
        var actualUnderMaintenances = await umControlAdapter.GetScheduledUnderMaintenanceInformation();

        // Assert
        Assert.True(OldNewSiteIdMappers.All(kv => actualUnderMaintenances.ContainsKey(kv.Key)));
        Assert.True(actualUnderMaintenances.All(um => !um.Value.IsUnderMaintenanceTime));
        Assert.True(actualUnderMaintenances.All(um => um.Value.From == "0:00"));
        Assert.True(actualUnderMaintenances.All(um => um.Value.To == "0:00"));
    }
}