using System.Globalization;
using Microsoft.Extensions.Options;
using ONEbook.UM.Api.Configurations;
using ONEbook.UM.Api.Models;

namespace ONEbook.UM.Api.Services;

public interface IFakeUMControlAdapter
{
    UnderMaintenanceTime GetUnderMaintenance();
}

public class FakeUMControlAdapter(IOptions<FakeUMConfiguration> options) : IFakeUMControlAdapter
{
    private readonly FakeUMConfiguration options = options.Value;

    public UnderMaintenanceTime GetUnderMaintenance()
    {
        var now = DateTimeOffset.UtcNow.AddHours(8);

        if (!options.Enable)
        {
            return null;
        }

        foreach (var minute in options.MinutesCheck)
        {
            if (minute == now.Minute || (now.Minute > minute && (now.Minute - minute) * 60 < options.Period))
            {
                var startFakeTime = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, minute, 0, TimeSpan.Zero);
                return new UnderMaintenanceTime(
                    true,
                    startFakeTime.ToString(CultureInfo.InvariantCulture),
                    startFakeTime.AddSeconds(options.Period).ToString(CultureInfo.InvariantCulture));
            }
        }

        return new UnderMaintenanceTime(
            false,
            DateTimeOffset.MinValue.Date.ToString(CultureInfo.InvariantCulture),
            DateTimeOffset.MinValue.Date.ToString(CultureInfo.InvariantCulture));
    }
}
