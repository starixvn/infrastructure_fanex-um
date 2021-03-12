namespace ONEbook.UM.Console
{
    using ONEbook.UM.Services.Impl;
    using System;

    internal class Program
    {
        private static void Main()
        {
            var maintenanceService = new MaintenanceService();

            var uminfo = maintenanceService.GetUnderMaintenanceTime();
            var isUM = maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

            Console.WriteLine(isUM);

            Console.WriteLine(
                string.Join(
                "\r\n",
                "------------",
                uminfo.IsUnderMaintenanceTime,
                uminfo.From,
                uminfo.To,
                uminfo.ConnectionResult.IsOk,
                uminfo.ConnectionResult.Message));
        }
    }
}