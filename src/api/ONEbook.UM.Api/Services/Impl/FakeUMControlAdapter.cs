namespace ONEbook.UM.Api.Services.Impl
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using ONEbook.UM.Api.Models;

    public static class FakeUMControlAdapter
    {
        public static UnderMaintenanceTime GetUnderMaintenance()
        {
            bool enableFakeUM = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableFakeUM"] ?? "false");
            int umPeriod = Convert.ToInt32(ConfigurationManager.AppSettings["UMPeriod"] ?? "300");
            var minutesCheckUM = (ConfigurationManager.AppSettings["MinutesCheckUM"] ?? "15,45").Split(',').Select(int.Parse).ToList();

            var now = DateTime.UtcNow.AddHours(8);

            if (!enableFakeUM)
            {
                return null;
            }

            foreach (var minute in minutesCheckUM)
            {
                if (minute == now.Minute || (now.Minute > minute && ((now.Minute - minute) * 60 < umPeriod)))
                {
                    var startFakeTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, minute, 0);
                    return new UnderMaintenanceTime(
                        true,
                        startFakeTime.ToString(CultureInfo.InvariantCulture),
                        startFakeTime.AddSeconds(umPeriod).ToString(CultureInfo.InvariantCulture),
                        0);
                }
            }

            return new UnderMaintenanceTime(false, string.Empty, string.Empty);
        }
    }
}