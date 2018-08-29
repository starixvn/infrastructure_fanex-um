namespace ONEbook.UM.Configurations
{
    public interface IUMClientConfiguration
    {
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
    }
}