namespace ONEbook.UM.Core.Configurations
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    internal class IPElement : ConfigurationElement
    {
        private const string ValueName = "Value";
        private const string FromName = "From";
        private const string ToName = "To";

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed!"),
        ConfigurationProperty(ValueName, IsRequired = false)]
        public string Value
        {
            get
            {
                return (string)this[ValueName];
            }

            set
            {
                this[ValueName] = value;
            }
        }

        [ConfigurationProperty(FromName, IsRequired = false)]
        public string From
        {
            get { return (string)this[FromName]; }
            set { this[FromName] = value; }
        }

        [ConfigurationProperty(ToName, IsRequired = false)]
        public string To
        {
            get { return (string)this[ToName]; }
            set { this[ToName] = value; }
        }
    }
}