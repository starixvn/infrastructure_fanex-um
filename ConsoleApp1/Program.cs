using System;
using ONEbook.UM.AspNetCore.Services;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ONEbook.UM.Core.Configurations.UMClientConfiguration.InitConfigurationRoot("");
            var maintenanceService = new MaintenanceService();
            var umInfo = maintenanceService.GetUnderMaintenanceTime().GetAwaiter().GetResult();
        }
    }
}