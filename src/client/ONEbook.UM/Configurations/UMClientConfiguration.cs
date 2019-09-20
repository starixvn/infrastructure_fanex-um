namespace ONEbook.UM.Configurations
{
    public sealed class UMClientConfiguration
    {
        public UMClientConfiguration()
        {
        }

        public UMClientConfiguration(
            string serviceUri,
            bool enableCheckUm,
            string secretKey,
            bool enableUnscheduledUm,
            string umFrom,
            string umTo,
            string umPageUrl,
            int siteId,
            int resultCacheDuration,
            string umLoggingCategory,
            string errorUmPageUrl)
        {
            ServiceUri = serviceUri;
            EnableCheckUM = enableCheckUm;
            SecretKey = secretKey;
            EnableUnscheduledUM = enableUnscheduledUm;
            UMFrom = umFrom;
            UMTo = umTo;
            UMPageUrl = umPageUrl;
            SiteId = siteId;
            ResultCacheDuration = resultCacheDuration;
            UMLoggingCategory = umLoggingCategory;
            ErrorUMPageUrl = errorUmPageUrl;
        }

        public string ServiceUri { get; set; }

        public bool EnableCheckUM { get; set; } = true;

        public string SecretKey { get; set; } = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=";

        public bool EnableUnscheduledUM { get; set; }

        public string UMFrom { get; set; }

        public string UMTo { get; set; }

        public string UMPageUrl { get; set; } = "~/UserLogin/Common/Offline";

        public int SiteId { get; set; }

        public int ResultCacheDuration { get; set; } = 60;

        public string UMLoggingCategory { get; set; } = "ONEbook-UM";

        public string ErrorUMPageUrl { get; set; }
    }
}