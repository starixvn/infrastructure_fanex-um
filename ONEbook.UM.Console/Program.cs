namespace ONEbook.UM.Console
{
    using ONEbook.UM.Services.Impl;
    using System;
    using Configurations;
    using RestSharp;

    internal class Program
    {
        private static void Main()
        {
            //UMClientManager.WhenError((message) => Console.WriteLine(message));
            var maintenanceService = new MaintenanceService();

            var uminfo = maintenanceService.GetUnderMaintenanceTime();

            var isUM = maintenanceService.GetUnderMaintenanceTime().IsUnderMaintenanceTime;

            //var isUM1 = maintenanceService.IsUnderMaintenance();

            Console.WriteLine(
                string.Join(
                "\r\n",
                "------------",
                uminfo.IsUnderMaintenanceTime,
                uminfo.From,
                uminfo.To,
                uminfo.ConnectionResult.IsOk,
                uminfo.ConnectionResult.Message));

            ////var restClient = new RestClient(new Uri("http://localhost:58359/api"));
            ////var request = new RestRequest("/Values?password=3434343&securitycode=344545&rrr=44", Method.GET);

            ////IRestResponse response = restClient.Execute(request);
        }

    }
}