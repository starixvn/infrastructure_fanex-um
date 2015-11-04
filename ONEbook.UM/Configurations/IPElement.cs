namespace ONEbook.UM.Configurations
{
    using System.Configuration;

    public class IPElement : ConfigurationElement
    {
        private const string ValueName = "Value";

        [ConfigurationProperty(ValueName, IsRequired = true)]
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