namespace ONEbook.UM.Configurations
{
    public interface IUMClientConfiguration
    {
#pragma warning disable S3996 // URI properties should not be strings
        string ServiceUri { get; }

        bool EnableCheckUM { get; }

        string SecretKey { get; }

        bool EnableUnscheduledUM { get; }

        string UMFrom { get; }

        string UMTo { get; }

        string UMPageUrl { get; }

        int SiteId { get; }

        int ResultCacheDuration { get; }

        string UMLoggingCategory { get; }

        string ErrorUMPageUrl { get; }

#pragma warning restore S3996 // URI properties should not be strings
    }
}