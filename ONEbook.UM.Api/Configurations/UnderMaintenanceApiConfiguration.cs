namespace ONEbook.UM.Api.Configurations
{
    using System.Configuration;

    public class UnderMaintenanceApiConfiguration : ConfigurationSection
    {
        private static UnderMaintenanceApiConfiguration settings
            = ConfigurationManager.GetSection("ONEbook.UM.Api") as UnderMaintenanceApiConfiguration;

        public static UnderMaintenanceApiConfiguration Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("SecretKey", IsRequired = false, DefaultValue = "9yBNYA5Fnv2V58+pKLvYpFwV5BMkIh9NNTxPQk6Sbss=")]
        public string SecretKey
        {
            get { return (string)this["SecretKey"]; }
            set { this["SecretKey"] = value; }
        }
    }
}