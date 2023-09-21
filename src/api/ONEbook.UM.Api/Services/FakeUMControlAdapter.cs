using System.Globalization;
using Microsoft.Extensions.Options;
using ONEbook.UM.Api.Configurations;
using ONEbook.UM.Api.Models;

namespace ONEbook.UM.Api.Services;

public interface IFakeUMControlAdapter
{
    UnderMaintenanceTime GetUnderMaintenance();
}

public class FakeUMControlAdapter : IFakeUMControlAdapter
{
    private readonly FakeUMConfiguration options;

    public FakeUMControlAdapter(IOptions<FakeUMConfiguration> options)
    {
        this.options = options.Value;
    }

    public UnderMaintenanceTime GetUnderMaintenance()
    {
        var now = DateTime.UtcNow.AddHours(8);

        if (!options.Enable)
        {
            return null;
        }

        foreach (var minute in options.MinutesCheck)
        {
            if (minute == now.Minute || (now.Minute > minute && (now.Minute - minute) * 60 < options.Period))
            {
                var startFakeTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, minute, 0);
                return new UnderMaintenanceTime(
                    true,
                    startFakeTime.ToString(CultureInfo.InvariantCulture),
                    startFakeTime.AddSeconds(options.Period).ToString(CultureInfo.InvariantCulture),
                    0);
            }
        }

        return new UnderMaintenanceTime(
            false,
            DateTime.MinValue.ToString(CultureInfo.InvariantCulture),
            DateTime.MinValue.ToString(CultureInfo.InvariantCulture));
    }
}
