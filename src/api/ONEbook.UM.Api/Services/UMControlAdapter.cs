using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using ONEbook.UM.Api.Configurations;
using ONEbook.UM.Api.Constants;
using ONEbook.UM.Api.Models;

namespace ONEbook.UM.Api.Services;

public interface IUMControlAdapter
{
    Task<UnderMaintenanceTime> GetUnderMaintenanceTime(int siteId);

    Task<IDictionary<int, UnderMaintenanceTime>> GetScheduledUnderMaintenanceInformation();

    Task<IDictionary<int, UnderMaintenanceTime>> GetAllUnderMaintenanceTime();
}

public class UMControlAdapter : IUMControlAdapter
{
    private readonly UnderMaintenanceApiConfiguration options;
    private readonly IFakeUMControlAdapter fakeUMControlAdapter;
    private readonly HttpClient httpClient;
    private readonly Fanex.Logging.ILogger logger;
    public const int SUCCESS_ECODE = 0;
    public const int NO_FOUND_ECODE = 1;
    public const int NO_CONNECTION_ECODE = 2;

    public UMControlAdapter(
        IOptions<UnderMaintenanceApiConfiguration> options,
        IFakeUMControlAdapter fakeUMControlAdapter,
        HttpClient httpClient,
        Fanex.Logging.ILogger logger)
    {
        this.options = options.Value;
        this.fakeUMControlAdapter = fakeUMControlAdapter;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    /// <summary>
    /// Old and New SiteID for UM Tool
    /// </summary>
    private static readonly IDictionary<int, int> OldNewSiteIdMappers = new Dictionary<int, int>
        {
            { (int)Sites.MBCAgencySite, (int)Sites.MBCAgencySite },
            { (int)Sites.IndoAgencySite, (int)Sites.IndoAgencySite },
            { (int)Sites._789yAgencySite, (int)Sites._789yAgencySite },
            { (int)Sites.AccountSite, (int)Sites.AccountSite },
            { (int)Sites.LRF, (int)Sites.LRF },
            { (int)Sites.OWA, (int)Sites.OWA },
            { (int)Sites.Aff12Bet, (int)Sites.Aff12Bet },
            { (int)Sites.RMS, (int)Sites.RMS },
            { (int)Sites.OddsFeed, (int)Sites.OddsFeed },
            { (int)Sites._789y, 4100300 },
            { (int)Sites.Test, (int)Sites.Test },
        };

    public async Task<UnderMaintenanceTime> GetUnderMaintenanceTime(int siteId)
    {
        var fakeUMInfo = fakeUMControlAdapter.GetUnderMaintenance();

        if (fakeUMInfo != null)
        {
            return fakeUMInfo;
        }

        if (options.EnableUnscheduledUM)
        {
            return new UnderMaintenanceTime(true, options.UMFrom, options.UMTo);
        }

        if (options.EnableCheckUM)
        {
            var (isUnderUM, startTime, endTime, errorCode) = await FetchUMInformation(siteId);
            return new UnderMaintenanceTime(isUnderUM, startTime.ToString("HH:mm"), endTime.ToString("HH:mm"), errorCode);
        }

        return new UnderMaintenanceTime(false, "0:00", "0:00");
    }

    public async Task<IDictionary<int, UnderMaintenanceTime>> GetAllUnderMaintenanceTime()
    {
        var result = new Dictionary<int, UnderMaintenanceTime>();
        foreach (var siteId in OldNewSiteIdMappers.Keys)
        {
            result.Add(siteId, await GetUnderMaintenanceTime(siteId));
        }
        return result;
    }

    public async Task<IDictionary<int, UnderMaintenanceTime>> GetScheduledUnderMaintenanceInformation()
    {
        var siteIds = OldNewSiteIdMappers.Keys;

        var fakeUMInfo = fakeUMControlAdapter.GetUnderMaintenance();

        if (fakeUMInfo != null)
        {
            return siteIds.ToDictionary(siteId => siteId, _ => fakeUMInfo);
        }

        var result = new Dictionary<int, UnderMaintenanceTime>();
        foreach (var siteId in siteIds)
        {
            result.Add(siteId, await GetScheduledUnderMaintenance(siteId));
        }

        return result;
    }

    private async Task<UnderMaintenanceTime> GetScheduledUnderMaintenance(int siteId)
    {
        if (options.EnableUnscheduledUM)
        {
            return new UnderMaintenanceTime(true, options.UMFrom, options.UMTo);
        }


        var (isUnderUM, startTime, endTime, errorCode) = await FetchUMInformation(siteId);

        if (startTime.Date.Year != DateTime.MinValue.Year)
        {
            return new UnderMaintenanceTime(
               isUnderUM,
               startTime.ToString(CultureInfo.InvariantCulture),
               endTime.ToString(CultureInfo.InvariantCulture),
               errorCode);
        }

        return new UnderMaintenanceTime(false, "0:00", "0:00");
    }

    private async Task<(bool, DateTime, DateTime, int)> FetchUMInformation(int siteId)
    {
        var isUM = false;
        var startTime = new DateTime();
        var endTime = new DateTime();
        var errorCode = NO_CONNECTION_ECODE;

        foreach (var url in options.UMServiceURL)
        {
            errorCode = NO_FOUND_ECODE;
            var uriString = Path.Combine(url, siteId.ToString());
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString))
                { Headers = { { "UMClientVersion", "1.0.21151.1321" } } };
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var responseString = new StreamReader(contentStream, Encoding.UTF8).ReadToEnd();
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        var serializer = new XmlSerializer(typeof(UMStageMessageSite));
                        var stageMessageSite = (UMStageMessageSite)serializer.Deserialize(new StringReader(responseString));
                        var messageSite = stageMessageSite.MessageSites[0];
                        isUM = true;
                        startTime = messageSite.StartTime.AddHours(8.0);
                        endTime = messageSite.EndTime.AddHours(8.0);
                        errorCode = SUCCESS_ECODE;
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                var message = string.Join("\r\n", $"Checking {uriString} failed!", ex.Message);
                await logger.ErrorAsync(message, ex);
            }
        }

        return (isUM, startTime, endTime, errorCode);
    }
}
