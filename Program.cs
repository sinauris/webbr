using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Webbr
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();
        private static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(conf => conf.AddEnvironmentVariables())
            .UseStartup<Startup>()
            .UseUrls("http://localhost:5000");
    }
}