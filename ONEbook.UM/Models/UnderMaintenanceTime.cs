namespace ONEbook.UM.Models
{
    public class UnderMaintenanceTime
    {
        public UnderMaintenanceTime(
            bool isUnderMaintenanceTime,
            string from,
            string to)
        {
            IsUnderMaintenanceTime = isUnderMaintenanceTime;
            From = from;
            To = to;
        }

        public string From { get; private set; }

        public string To { get; private set; }

        public bool IsUnderMaintenanceTime { get; private set; }
    }
}