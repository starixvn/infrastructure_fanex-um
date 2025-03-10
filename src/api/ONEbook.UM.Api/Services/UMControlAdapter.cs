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

public class UMControlAdapter(
    IOptions<UnderMaintenanceApiConfiguration> options,
    IFakeUMControlAdapter fakeUmControlAdapter,
    HttpClient httpClient,
    Fanex.Logging.ILogger logger)
    : IUMControlAdapter
{
    private readonly UnderMaintenanceApiConfiguration options = options.Value;
    public const int SUCCESS_ECODE = 0;
    public const int NO_FOUND_ECODE = 1;
    public const int NO_CONNECTION_ECODE = 2;

    /// <summary>
    /// Old and New SiteID for UM Tool
    /// </summary>
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
            { (int)Sites.DW, (int)Sites.DW },
            { (int)Sites.OddsFeed, (int)Sites.OddsFeed },
            { (int)Sites.CRM, (int)Sites.CRM },
            { (int)Sites.LicenseeAccountsAPI, (int)Sites.LicenseeAccountsAPI },
            { (int)Sites._789y, 4100300 },
            { (int)Sites.Test, (int)Sites.Test }
        };

    public async Task<UnderMaintenanceTime> GetUnderMaintenanceTime(int siteId)
    {
        var fakeUMInfo = fakeUmControlAdapter.GetUnderMaintenance();

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

        var fakeUMInfo = fakeUmControlAdapter.GetUnderMaintenance();

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
        var serviceHealth = new Dictionary<string, bool>();
        var responseString = string.Empty;

        foreach (var url in options.UMServiceURL)
        {
            var uriString = Path.Combine(url, siteId.ToString());
            try
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(uriString))
                { Headers = { { "UMClientVersion", "1.0.21151.1321" } } };
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    serviceHealth.Add(url, true);
                    await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    responseString = await new StreamReader(contentStream, Encoding.UTF8).ReadToEndAsync();
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        isUM = true;
                        break;
                    }
                }
                else
                {
                    serviceHealth.Add(url, false);
                }
            }
            catch (Exception ex)
            {
                serviceHealth.Add(url, false);
                var message = string.Join("\r\n", $"Checking {uriString} failed!", ex.Message);
                await logger.ErrorAsync(message, ex);
            }
        }

        if (!isUM)
        {
            return (false, new DateTime(), new DateTime(), serviceHealth.ContainsValue(true) ? NO_FOUND_ECODE : NO_CONNECTION_ECODE);
        }

        var serializer = new XmlSerializer(typeof(UMStageMessageSite));
        var stageMessageSite = (UMStageMessageSite)serializer.Deserialize(new StringReader(responseString));
        var messageSite = stageMessageSite.MessageSites[0];
        var startTime = messageSite.StartTime;
        var endTime = messageSite.EndTime;
        var utcNow = DateTime.UtcNow;
        if (utcNow.Subtract(startTime).TotalSeconds >= 0.0 && endTime.Subtract(utcNow).TotalSeconds >= 0.0)
        {
            return (true, startTime.AddHours(8.0), endTime.AddHours(8.0), SUCCESS_ECODE);
        }

        return (false, startTime.AddHours(8.0), endTime.AddHours(8.0), SUCCESS_ECODE);
    }
}
