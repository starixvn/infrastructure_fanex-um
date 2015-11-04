namespace Fanex.UM.Client.Configurations
{
    using System.Configuration;

    public class UMClientConfiguration : ConfigurationSection
    {
        private const string SiteIdName = "SiteId";
        private const string ServiceUrisName = "ServiceUris";
        private const string EnableCheckUMName = "EnableCheckUM";
        private const string UMAddressName = "UMAddress";

        private static UMClientConfiguration settings
            = ConfigurationManager.GetSection("Fanex.UM.Client") as UMClientConfiguration;

        public static UMClientConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty(ServiceUrisName, IsRequired = true)]
        public string ServiceUris
        {
            get { return (string)this[ServiceUrisName]; }
            set { this[ServiceUrisName] = value; }
        }

        [ConfigurationProperty(EnableCheckUMName, IsRequired = false, DefaultValue = true)]
        public bool EnableCheckUM
        {
            get { return (bool)this[EnableCheckUMName]; }
            set { this[EnableCheckUMName] = value; }
        }

        [ConfigurationProperty(UMAddressName, IsRequired = true)]
        public string UMAddress
        {
            get { return (string)this[UMAddressName]; }
            set { this[UMAddressName] = value; }
        }
    }
}