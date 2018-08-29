namespace ONEbook.UM.Api.Models
{
    public class UnderMaintenanceTime
    {
        public UnderMaintenanceTime(
            bool isUnderMaintenanceTime,
            string from,
            string to,
            int errorCode = 0)
        {
            IsUnderMaintenanceTime = isUnderMaintenanceTime;
            From = from;
            To = to;
            ErrorCode = errorCode;
        }

        public string From { get; }

        public string To { get; }

        public bool IsUnderMaintenanceTime { get; }

        public int ErrorCode { get; }
    }
}