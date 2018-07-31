using System;
using ONEbook.UM.AspNetCore.Services;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var maintenanceService = new MaintenanceService();
            maintenanceService.GetUnderMaintenanceTime().GetAwaiter().GetResult();
        }
    }
}