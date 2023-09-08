namespace ONEbook.UM.Site.Configurations
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class UMSiteConfiguration : ConfigurationSection
    {
        private const string AllowedLoginAccountsConfigName = "AllowedLoginAccounts";
        private const string ServiceUriName = "ServiceUri";

        private static IList<string> _allowedLoginAccounts;

        public static UMSiteConfiguration Settings { get; } = ConfigurationManager.GetSection("ONEbook.UM.Site") as UMSiteConfiguration;

        public static IList<string> AllowedLoginAccounts
        {
            get
            {
                if (_allowedLoginAccounts == null)
                {
                    var allowedAccount = Settings.AllowedLoginAccountsConfig;
                    _allowedLoginAccounts = allowedAccount.Split(',').ToList();
                }

                return _allowedLoginAccounts;
            }
        }

        [ConfigurationProperty(AllowedLoginAccountsConfigName, IsRequired = true)]
        public string AllowedLoginAccountsConfig
        {
            get => (string)this[AllowedLoginAccountsConfigName];
            set => this[AllowedLoginAccountsConfigName] = value;
        }

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Reviewed!"),
            ConfigurationProperty(ServiceUriName, IsRequired = true)]
        public string ServiceUri
        {
            get => (string)this[ServiceUriName];
            set => this[ServiceUriName] = value;
        }
    }
}