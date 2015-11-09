namespace ONEbook.UM.Api.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Fanex.Common.Helper;
    using Sunplus.UMControllerLibrary;

    public class MaintenanceService : IMaintenanceService
    {
        public bool IsValidUMSecretToken(Sites siteId, string secretToken)
        {
            Check.Argument.IsNotEmpty(secretToken, "secretToken");
            Check.Argument.IsNotNegativeOrZero((int)siteId, "siteId");

            var validToken = GenerateToken(siteId);

            return validToken.Equals(secretToken, StringComparison.Ordinal);
        }

        private string GenerateToken(Sites siteId)
        {
            var today = DateTime.UtcNow.ToShortDateString();
            var key = string.Format("{0}_{1}", siteId, today);

            using (MD5 md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));

                return Convert.ToBase64String(hash);
            }
        }

        public string GetUMSecretToken(Sites siteId)
        {
            Check.Argument.IsNotNegativeOrZero((int)siteId, "siteId");

            return GenerateToken(siteId);
        }

        public IDictionary<Sites, string> GetAllUMSecretToken()
        {
            var siteIds = Enum.GetValues(typeof(Sites)).Cast<Sites>();
            var siteTokens = new Dictionary<Sites, string>();

            foreach (Sites siteId in siteIds)
            {
                siteTokens.Add(siteId, GenerateToken(siteId));
            }

            return siteTokens;
        }
    }
}