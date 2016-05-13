namespace ONEbook.UM.Api.Configurations
{
    using System.Configuration;

    public class UnderMaintenanceApiConfiguration : ConfigurationSection
    {
        private const string EnableUnscheduledUMName = "EnableUnscheduledUM";
        private const string UMFromName = "UMFrom";
        private const string UMToName = "UMTo";
        private const string SecretKeyName = "SecretKey";
        private const string EnableCheckUMName = "EnableCheckUM";

        private static UnderMaintenanceApiConfiguration settings
            = ConfigurationManager.GetSection("ONEbook.UM.Api") as UnderMaintenanceApiConfiguration;

        public static UnderMaintenanceApiConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty(SecretKeyName, IsRequired = false, DefaultValue = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=")]
        public string SecretKey
        {
            get { return (string)this[SecretKeyName]; }
            set { this[SecretKeyName] = value; }
        }

        [ConfigurationProperty(EnableCheckUMName, IsRequired = false, DefaultValue = true)]
        public bool EnableCheckUM
        {
            get { return (bool)this[EnableCheckUMName]; }
            set { this[EnableCheckUMName] = value; }
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
    }
}