namespace ONEbook.UM.Configurations
{
    using System.Collections.Generic;
    using System.Configuration;

    public class UMClientConfiguration : ConfigurationSection
    {
        private const string SiteIdName = "SiteId";
        private const string ServiceUrisName = "ServiceUris";
        private const string EnableCheckUMName = "EnableCheckUM";
        private const string UMAddressName = "UMAddress";
        private const string AllowedIPListName = "AllowedIPs";
        private static IList<string> _allowIPs; 

        private static UMClientConfiguration settings
            = ConfigurationManager.GetSection("ONEbook.UM") as UMClientConfiguration;

        public static UMClientConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        public static IList<string> AllowedIPs
        {
            get
            {
                if (_allowIPs == null)
                {
                    _allowIPs = new List<string>();

                    foreach (IPElement ip in settings.IPList)
                    {
                        _allowIPs.Add(ip.Value);
                    }
                }

                return _allowIPs;
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

        [ConfigurationProperty(AllowedIPListName, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(IPCollection), AddItemName = "add")]
        internal IPCollection IPList
        {
            get
            {
                var ipList = (IPCollection)base[AllowedIPListName];

                return ipList;
            }
        }
    }
}