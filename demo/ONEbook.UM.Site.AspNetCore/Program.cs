namespace ONEbook.UM.Site.AspNetCore
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

#pragma warning disable S1118 // Utility classes should not have public constructors

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

#pragma warning restore S1118 // Utility classes should not have public constructors
}