namespace ONEbook.UM.Configurations
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "Reviewed!")]
    internal class IPCollection : ConfigurationElementCollection
    {
        private const string AllowAllName = "AllowAll";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        [ConfigurationProperty(AllowAllName, IsRequired = false, DefaultValue = true)]
        public bool AllowAll
        {
            get { return (bool)this[AllowAllName]; }
            set { this[AllowAllName] = value; }
        }

        public IPElement this[int index]
        {
            get
            {
                return (IPElement)BaseGet(index);
            }

            set
            {
                if (this.BaseGet(index) != null)
                {
                    this.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        public void Add(IPElement mappingElement)
        {
            this.BaseAdd(mappingElement);
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new IPElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var ipElement = element as IPElement;

            return ipElement.Value;
        }
    }
}