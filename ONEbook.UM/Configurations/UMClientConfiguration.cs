namespace ONEbook.UM.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using Sunplus.UMControllerLibrary;

    public class UMClientConfiguration : ConfigurationSection
    {
        private const string SiteIdName = "SiteId";
        private const string ServiceUrisName = "ServiceUris";
        private const string EnableCheckUMName = "EnableCheckUM";
        private const string EnableUMNowName = "EnableUMNow";
        private const string UMFromName = "UMFrom";
        private const string UMToName = "UMTo";
        private const string UMAddressName = "UMAddress";
        private const string AllowedIPListName = "AllowedIPs";
        private static IList<Tuple<string, string>> _allowIPs;

        private static UMClientConfiguration settings
            = ConfigurationManager.GetSection("ONEbook.UM") as UMClientConfiguration;

        public static UMClientConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed!")]
        public static IList<Tuple<string, string>> AllowedIPs
        {
            get
            {
                if (_allowIPs == null)
                {
                    _allowIPs = new List<Tuple<string, string>>();

                    foreach (IPElement ip in settings.IPList)
                    {
                        if (!string.IsNullOrEmpty(ip.Value))
                        {
                            _allowIPs.Add(new Tuple<string, string>(ip.Value, ip.Value));
                        }

                        if (!string.IsNullOrEmpty(ip.From) && !string.IsNullOrEmpty(ip.To))
                        {
                            _allowIPs.Add(new Tuple<string, string>(ip.From, ip.To));
                        }
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

        [ConfigurationProperty("SecretKey", IsRequired = false, DefaultValue = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=")]
        public string SecretKey
        {
            get { return (string)this["SecretKey"]; }
            set { this["SecretKey"] = value; }
        }

        [ConfigurationProperty(EnableUMNowName, IsRequired = false, DefaultValue = false)]
        public bool EnableUMNow
        {
            get { return (bool)this[EnableUMNowName]; }
            set { this[EnableUMNowName] = value; }
        }

        [ConfigurationProperty(UMFromName, IsRequired = false)]
        public string UMFrom
        {
            get { return (string)this[UMFromName]; }
            set { this[UMFromName] = value; }
        }

        [ConfigurationProperty(UMToName, IsRequired = false)]
        public string UMTo
        {
            get { return (string)this[UMToName]; }
            set { this[UMToName] = value; }
        }

        [ConfigurationProperty(SiteIdName, IsRequired = true)]
        public int SiteId
        {
            get { return (int)this[SiteIdName]; }
            set { this[SiteIdName] = value; }
        }
    }
}