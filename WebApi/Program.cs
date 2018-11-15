using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NewWorld.Integrations.WebApi.Configuration;

namespace NewWorld.Integrations.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Starting up!...");
                CreateWebHostBuilder(args).Build().Run();
                logger.Debug("Shutting down...");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex);
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            // All I want to do is add a config file for setting the hosting ports
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("hostsettings.json", optional: false)
                .Build();

            return new WebHostBuilder()
                .UseConfiguration(configuration)
                .ConfigureDefaultLoggging()
                .UseDefaultWebHostConfiguration<Startup>(args);
        }
    }
}
