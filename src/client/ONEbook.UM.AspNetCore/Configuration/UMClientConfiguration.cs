namespace ONEbook.UM.AspNetCore.Configuration
{
    using System;
    using System.ComponentModel;
    using Microsoft.Extensions.Configuration;
    using ONEbook.UM.Configurations;

    public class UMClientConfiguration : IUMClientConfiguration
    {
        private readonly IConfiguration configuration;

        public UMClientConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

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

        private T GetValue<T>(string key, T defaultValue)
        {
            var value = configuration[key];

            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return GetValue<T>(key);
        }

        private T GetValue<T>(string key)
        {
            var value = configuration[key];

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}