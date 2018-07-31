namespace ONEbook.UM.Core.Models
{
    public class UnderMaintenanceTime
    {
        public UnderMaintenanceTime()
        {
            ConnectionResult = new ConnectionResult();
        }

        public UnderMaintenanceTime(
            bool isUnderMaintenanceTime,
            string from,
            string to,
            ConnectionResult connectionResult = null,
            int errorCode = 0)
        {
            IsUnderMaintenanceTime = isUnderMaintenanceTime;
            From = from;
            To = to;
            ConnectionResult = connectionResult ?? new ConnectionResult();
            ErrorCode = errorCode;
        }

        public string From { get; set; }

        public string To { get; set; }

        public bool IsUnderMaintenanceTime { get; set; }

        public int ErrorCode { get; set; }

        public ConnectionResult ConnectionResult { get; set; }

        public static UnderMaintenanceTime CreateFailedConnection(string message = "Connection Error")
            => new UnderMaintenanceTime(
                false,
                "0:00",
                "0.00",
                new ConnectionResult(false, message));
    }
}