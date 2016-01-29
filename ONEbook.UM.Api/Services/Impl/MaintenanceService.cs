namespace ONEbook.UM.Api.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Fanex.Common.Helper;
    using ONEbook.UM.Api.Configurations;
    using ONEbook.UM.Api.Models;
    using Sunplus.UMControllerLibrary;

    public class MaintenanceService : IMaintenanceService
    {
        public bool IsValidUMToken(Sites siteId, string token)
        {
            Check.Argument.IsNotEmpty(token, "token");
            Check.Argument.IsNotNegativeOrZero((int)siteId, "siteId");

            var validToken = GenerateToken(siteId);

            return validToken.Equals(token, StringComparison.Ordinal);
        }

        private string GenerateToken(Sites siteId)
        {
            var today = DateTime.UtcNow.ToShortDateString();
            var key = string.Format("{0}_{1}_{2}", siteId, today, UnderMaintenanceApiConfiguration.Settings.SecretKey);

            using (MD5 md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

                return HttpServerUtility.UrlTokenEncode(hash);
            }
        }

        public string GetUMToken(Sites siteId)
        {
            Check.Argument.IsNotNegativeOrZero((int)siteId, "siteId");

            return GenerateToken(siteId);
        }

        public IDictionary<Sites, SiteToken> GetAllUMTokens()
        {
            var siteIds = Enum.GetValues(typeof(Sites)).Cast<Sites>();
            var siteTokens = new Dictionary<Sites, SiteToken>();

            foreach (Sites siteId in siteIds)
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
}