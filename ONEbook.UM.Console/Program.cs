namespace ONEbook.UM.Console
{
    using ONEbook.UM.Services.Impl;
    using System;
    using Configurations;

    internal class Program
    {
        private static void Main()
        {
            UMClientManager.WhenError((message) => Console.WriteLine(message));

            var uminfo = (new UMControlAdapter()).GetUnderMaintenanceTime();

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