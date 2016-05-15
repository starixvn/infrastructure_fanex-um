namespace ONEbook.UM.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    public class UMClientConfiguration : ConfigurationSection
    {
        private const string SiteIdName = "SiteId";
        private const string ServiceUrisName = "ServiceUris";
        private const string EnableCheckUMName = "EnableCheckUM";
        private const string EnableUnscheduledUMName = "EnableUnscheduledUM";
        private const string UMFromName = "UMFrom";
        private const string UMToName = "UMTo";
        private const string AllowedIPListName = "AllowedIPs";
        private const string SecretKeyName = "SecretKey";
        private const string UMPageUrlName = "UMPageUrl";
        private const string UMResultCacheDurationName = "UMResultCacheDuration";

        private static IList<Tuple<string, string>> _allowIPs;

        private static UMClientConfiguration settings;

        public static UMClientConfiguration Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = ConfigurationManager.GetSection("ONEbook.UM") as UMClientConfiguration;
                }

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

        [ConfigurationProperty(SecretKeyName, IsRequired = false, DefaultValue = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=")]
        public string SecretKey
        {
            get { return (string)this[SecretKeyName]; }
            set { this[SecretKeyName] = value; }
        }

        [ConfigurationProperty(EnableUnscheduledUMName, IsRequired = false, DefaultValue = false)]
        public bool EnableUnscheduledUM
        {
            get { return (bool)this[EnableUnscheduledUMName]; }
            set { this[EnableUnscheduledUMName] = value; }
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

        [ConfigurationProperty(UMPageUrlName, IsRequired = false, DefaultValue = "~/UserLogin/Common/Offline")]
        public string UMPageUrl
        {
            get { return (string)this[UMPageUrlName]; }
            set { this[UMPageUrlName] = value; }
        }

        [ConfigurationProperty(SiteIdName, IsRequired = true, DefaultValue = 4120)]
        public int SiteId
        {
            get { return (int)this[SiteIdName]; }
            set { this[SiteIdName] = value; }
        }

        [ConfigurationProperty(UMResultCacheDurationName, IsRequired = false, DefaultValue = 60)]
        public int ResultCacheDuration
        {
            get { return (int)this[UMResultCacheDurationName]; }
            set { this[UMResultCacheDurationName] = value; }
        }
    }
}