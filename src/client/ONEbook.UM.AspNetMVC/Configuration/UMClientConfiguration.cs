namespace ONEbook.UM.AspNetMVC.Configuration
{
    using System.ComponentModel;
    using System.Configuration;
    using ONEbook.UM.Configurations;

    public class UMClientConfiguration : IUMClientConfiguration
    {
        public UMClientConfiguration()
        {
            ServiceUri = GetValue<string>("ONEbook.UM:ServiceUri");
            EnableCheckUM = GetValue("ONEbook.UM:EnableCheckUM", true);
            SecretKey = GetValue("ONEbook.UM:SecretKey", "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=");
            EnableUnscheduledUM = GetValue("ONEbook.UM:EnableUnscheduledUM", false);
            UMFrom = GetValue<string>("ONEbook.UM:UMFrom");
            UMTo = GetValue<string>("ONEbook.UM:UMTo");
            UMPageUrl = GetValue("ONEbook.UM:UMPageUrl", "~/UserLogin/Common/Offline");
            SiteId = GetValue("ONEbook.UM:SiteId", 4120);
            ResultCacheDuration = GetValue("ONEbook.UM:ResultCacheDuration", 60);
            UMLoggingCategory = GetValue("ONEbook.UM:UMLoggingCategory", "ONEbook-UM");
            ErrorUMPageUrl = GetValue<string>("ONEbook.UM:ErrorUMPageUrl");
        }

#pragma warning disable S3996 // URI properties should not be strings
        public string ServiceUri { get; }

        public bool EnableCheckUM { get; }

        public string SecretKey { get; }

        public bool EnableUnscheduledUM { get; }

        public string UMFrom { get; }

        public string UMTo { get; }

        public string UMPageUrl { get; }

        public int SiteId { get; }

        public int ResultCacheDuration { get; }

        public string UMLoggingCategory { get; }

        public string ErrorUMPageUrl { get; }
#pragma warning restore S3996 // URI properties should not be strings

        private static T GetValue<T>(string key, T defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return GetValue<T>(key);
        }

        private static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}