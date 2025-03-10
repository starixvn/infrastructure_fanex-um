using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using ONEbook.UM.Api.Configurations;
using ONEbook.UM.Api.Constants;
using ONEbook.UM.Api.Helpers;
using ONEbook.UM.Api.Models;

namespace ONEbook.UM.Api.Services;

public interface IUMTokenService
{
    bool IsValidUMToken(Sites siteId, string token);

    string GetUMToken(Sites siteId);

    IDictionary<Sites, SiteToken> GetAllUMTokens();
}

public class UMTokenService(IDateTimeHelper dateTimeHelper, IOptions<UnderMaintenanceApiConfiguration> options)
    : IUMTokenService
{
    private readonly IDateTimeHelper dateTimeHelper = dateTimeHelper ?? new DateTimeHelper();
    private readonly UnderMaintenanceApiConfiguration options = options.Value;

    public bool IsValidUMToken(Sites siteId, string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token));
        }

        if (siteId <= 0)
        {
            throw new ArgumentNullException(nameof(siteId));
        }

        var validToken = GenerateToken(siteId);

        return validToken.Equals(token, StringComparison.Ordinal);
    }

    private string GenerateToken(Sites siteId)
    {
        var today = dateTimeHelper.GetUtcNowShortDateString();
        var key = $"{siteId}_{today}_{options.SecretKey}";

        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

        return WebEncoders.Base64UrlEncode(hash);
    }

    public string GetUMToken(Sites siteId)
    {
        if (siteId <= 0)
        {
            throw new ArgumentNullException(nameof(siteId));
        }

        return GenerateToken(siteId);
    }

    public IDictionary<Sites, SiteToken> GetAllUMTokens()
    {
        var siteIds = Enum.GetValues(typeof(Sites)).Cast<Sites>();
        var siteTokens = new Dictionary<Sites, SiteToken>();

        foreach (var siteId in siteIds)
        {
            siteTokens.Add(
                siteId,
                new SiteToken
                {
                    SiteId = (int)siteId,
                    Token = GenerateToken(siteId)
                });
        }

        return siteTokens;
    }
}
