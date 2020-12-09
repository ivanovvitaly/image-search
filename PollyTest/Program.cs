using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PollyTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureAppConfiguration(
                           (context, builder) =>
                           {
                               var environment = context.HostingEnvironment;
                               builder
                                   .AddJsonFile("appsettings.json", false)
                                   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true);
                               builder.AddEnvironmentVariables();
                           })
                       .UseSerilog((hostingContext, services, loggerConfiguration) =>
                            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
                       .ConfigureWebHostDefaults(
                           webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}