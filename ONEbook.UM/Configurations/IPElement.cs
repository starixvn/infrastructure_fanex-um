namespace ONEbook.UM.Configurations
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    internal class IPElement : ConfigurationElement
    {
        private const string ValueName = "Value";

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed!"),
        ConfigurationProperty(ValueName, IsRequired = true)]
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
    }
}