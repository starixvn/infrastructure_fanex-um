namespace ONEbook.UM.Models
{
    public class UnderMaintenanceTime
    {
        public UnderMaintenanceTime()
        {
        }

        public UnderMaintenanceTime(
            bool isUnderMaintenanceTime,
            string from,
            string to)
        {
            IsUnderMaintenanceTime = isUnderMaintenanceTime;
            From = from;
            To = to;
        }

        public string From { get; set; }

        public string To { get; set; }

        public bool IsUnderMaintenanceTime { get; set; }
    }
}