namespace ONEbook.UM.Api.Models;

public class UnderMaintenanceTime(
    bool isUnderMaintenanceTime,
    string from,
    string to,
    int errorCode = 0)
{
    public string From { get; } = from;

    public string To { get; } = to;

    public bool IsUnderMaintenanceTime { get; } = isUnderMaintenanceTime;

    public int ErrorCode { get; } = errorCode;
}