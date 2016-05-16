namespace ONEbook.UM.Console
{
    using ONEbook.UM.Services.Impl;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var uminfo = (new UMControlAdapter()).GetUnderMaintenanceTime();
        }
    }
}