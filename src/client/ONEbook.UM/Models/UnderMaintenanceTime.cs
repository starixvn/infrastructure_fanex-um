namespace ONEbook.UM.Models
{
    public class UnderMaintenanceTime
    {
        /// <summary>
        /// Default constructor here for deserialize json.
        /// </summary>
        public UnderMaintenanceTime()
        {
            ConnectionResult = new ConnectionResult();
        }

        public UnderMaintenanceTime(
            bool isUnderMaintenanceTime,
            string from,
            string to,
            ConnectionResult connectionResult = null)
        {
            IsUnderMaintenanceTime = isUnderMaintenanceTime;
            From = from;
            To = to;
            ConnectionResult = connectionResult ?? new ConnectionResult();
        }

        public string From { get; set; }

        public string To { get; set; }

        public bool IsUnderMaintenanceTime { get; set; }

        public ConnectionResult ConnectionResult { get; set; }

        public static UnderMaintenanceTime CreateFailedConnection(string message = "Connection Error")
        {
            return new UnderMaintenanceTime(
                false,
                "0:00",
                "0.00",
                new ConnectionResult(false, message));
        }
    }
}