namespace ONEbook.UM.AspNetMVC
{
    using System.ComponentModel;
    using System.Configuration;
    using Configurations;

    public static class UMClientConfigurationExtension
    {
        public static UMClientConfiguration Initialize(this UMClientConfiguration umConfig)
        {
            umConfig = new UMClientConfiguration(
                GetValue<string>("ONEbook.UM:ServiceUri"),
                GetValue("ONEbook.UM:EnableCheckUM", true),
                GetValue("ONEbook.UM:SecretKey", "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss="),
                GetValue("ONEbook.UM:EnableUnscheduledUM", false),
                GetValue<string>("ONEbook.UM:UMFrom"),
                GetValue<string>("ONEbook.UM:UMTo"),
                GetValue("ONEbook.UM:UMPageUrl", "~/UserLogin/Common/Offline"),
                GetValue("ONEbook.UM:SiteId", 4120),
                GetValue("ONEbook.UM:ResultCacheDuration", 60),
                GetValue("ONEbook.UM:UMLoggingCategory", "ONEbook-UM"),
                GetValue<string>("ONEbook.UM:ErrorUMPageUrl")
            );

            return umConfig;
        }

#pragma warning disable S3996 // URI properties should not be strings

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