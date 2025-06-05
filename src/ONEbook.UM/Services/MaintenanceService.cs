namespace ONEbook.UM.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configurations;
using Helpers;
using Models;

public interface IMaintenanceService
{
    UnderMaintenanceTime GetUnderMaintenanceTime();

    Task<UnderMaintenanceTime> GetUnderMaintenanceTimeAsync();

    Dictionary<int, UnderMaintenanceTime> GetAllUnderMaintenanceTime();

    Task<Dictionary<int, UnderMaintenanceTime>> GetAllUnderMaintenanceTimeAsync();

    Dictionary<int, UnderMaintenanceTime> GetScheduledUnderMaintenance();

    Task<Dictionary<int, UnderMaintenanceTime>> GetScheduledUnderMaintenanceAsync();

    bool IsUnderMaintenance();

    Task<bool> IsUnderMaintenanceAsync();

    void RemoveUnderMaintenanceTimeCache();
}

public class MaintenanceService(
    IUMRestClient restClient,
    IUMCacheService cacheService,
    UMClientConfiguration configuration,
    IHttpContextHelper httpContextHelper = null)
    : IMaintenanceService
{
    private const string TokenName = "UMToken";
    private const string ByPassUmSessionKey = "ONEbook.UM.ByPassUM";
    private const string SiteUmCacheKey = "ONEbook.UM.Site.{0}";

    public bool ByPassUm
    {
        get
        {
            var isByPassUm = httpContextHelper?.GetSession(ByPassUmSessionKey);

            return !string.IsNullOrEmpty(isByPassUm) && isByPassUm == "1";
        }
        set => httpContextHelper?.SetSession(ByPassUmSessionKey, value ? "1" : "0");
    }

    public UnderMaintenanceTime GetUnderMaintenanceTime()
    {
        if (configuration.EnableUnscheduledUM)
        {
            return new UnderMaintenanceTime(true, configuration.UMFrom, configuration.UMTo);
        }

        if (!configuration.EnableCheckUM)
        {
            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }

        var cacheKey = string.Format(SiteUmCacheKey, configuration.SiteId);
        var umInfo = cacheService.Get<UnderMaintenanceTime>(cacheKey);

        if (umInfo != null)
        {
            return umInfo;
        }

        umInfo = GetUnderMaintenance();

        cacheService.Set(cacheKey, umInfo, TimeSpan.FromSeconds(configuration.ResultCacheDuration));

        return umInfo;

    }

    public async Task<UnderMaintenanceTime> GetUnderMaintenanceTimeAsync()
    {
        if (configuration.EnableUnscheduledUM)
        {
            return new UnderMaintenanceTime(true, configuration.UMFrom, configuration.UMTo);
        }

        if (!configuration.EnableCheckUM)
        {
            return new UnderMaintenanceTime(false, "0:00", "0:00");
        }

        var cacheKey = string.Format(SiteUmCacheKey, configuration.SiteId);
        var umInfo = cacheService.Get<UnderMaintenanceTime>(cacheKey);

        if (umInfo != null)
        {
            return umInfo;
        }

        umInfo = await GetUnderMaintenanceAsync().ConfigureAwait(false);

        cacheService.Set(cacheKey, umInfo, TimeSpan.FromSeconds(configuration.ResultCacheDuration));

        return umInfo;

    }

    public Dictionary<int, UnderMaintenanceTime> GetAllUnderMaintenanceTime()
        => restClient.ExecuteGet<Dictionary<int, UnderMaintenanceTime>>("UnderMaintenance/GetAll");

    public Task<Dictionary<int, UnderMaintenanceTime>> GetAllUnderMaintenanceTimeAsync()
        => restClient.ExecuteGetAsync<Dictionary<int, UnderMaintenanceTime>>("UnderMaintenance/GetAll");

    public Dictionary<int, UnderMaintenanceTime> GetScheduledUnderMaintenance()
        => restClient.ExecuteGet<Dictionary<int, UnderMaintenanceTime>>("UnderMaintenance/GetScheduledUnderMaintenance");

    public Task<Dictionary<int, UnderMaintenanceTime>> GetScheduledUnderMaintenanceAsync()
        => restClient.ExecuteGetAsync<Dictionary<int, UnderMaintenanceTime>>("UnderMaintenance/GetScheduledUnderMaintenance");

    public bool IsUnderMaintenance()
    {
        var token = httpContextHelper.GetValueFromQueryString(TokenName);
        var underMaintenanceTime = GetUnderMaintenanceTime();
        var nowIsNotUnderMaintenance = !underMaintenanceTime.IsUnderMaintenanceTime;

        if (nowIsNotUnderMaintenance)
        {
            return false;
        }

        if (ByPassUm)
        {
            return false;
        }

        var isValidToken = !string.IsNullOrEmpty(token) && IsTokenValid(token);

        if (!isValidToken)
        {
            return true;
        }

        ByPassUm = true;

        return false;

    }

    public async Task<bool> IsUnderMaintenanceAsync()
    {
        var token = httpContextHelper.GetValueFromQueryString(TokenName);
        var underMaintenanceTime = await GetUnderMaintenanceTimeAsync().ConfigureAwait(false);
        var nowIsNotUnderMaintenance = !underMaintenanceTime.IsUnderMaintenanceTime;

        if (nowIsNotUnderMaintenance)
        {
            return false;
        }

        if (ByPassUm)
        {
            return false;
        }

        var isValidToken = !string.IsNullOrEmpty(token) && IsTokenValid(token);

        if (!isValidToken)
        {
            return true;
        }

        ByPassUm = true;

        return false;

    }

    private bool IsTokenValid(string token)
    {
        try
        {
            return restClient
                .ExecuteGet<bool>($"token/validate?SiteId={configuration.SiteId}&Token={token}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    internal UnderMaintenanceTime GetUnderMaintenance()
    {
        try
        {
            return restClient.ExecuteGet<UnderMaintenanceTime>($"UnderMaintenance/get?siteId={configuration.SiteId}");
        }
        catch (Exception ex)
        {
            return UnderMaintenanceTime.CreateFailedConnection(ex.Message);
        }
    }

    internal async Task<UnderMaintenanceTime> GetUnderMaintenanceAsync()
    {
        try
        {
            return await restClient
                .ExecuteGetAsync<UnderMaintenanceTime>($"UnderMaintenance/get?siteId={configuration.SiteId}")
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return UnderMaintenanceTime.CreateFailedConnection(ex.Message);
        }
    }

    public void RemoveUnderMaintenanceTimeCache()
    {
        var cacheKey = string.Format(SiteUmCacheKey, configuration.SiteId);
        cacheService.Remove(cacheKey);
    }
}