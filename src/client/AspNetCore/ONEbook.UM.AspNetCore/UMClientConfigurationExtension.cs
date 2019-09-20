namespace ONEbook.UM.AspNetCore
{
    using System;
    using Configurations;
    using Microsoft.Extensions.Configuration;

    public static class UMClientConfigurationExtension
    {
        public static UMClientConfiguration Initialize(this UMClientConfiguration umConfig, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Bind("ONEbook.UM", umConfig);

            return umConfig;
        }
    }
}