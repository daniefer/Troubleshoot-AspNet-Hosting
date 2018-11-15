using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace NewWorld.Integrations.WebApi.Configuration
{
    public static class ConfigureLoggingExtensions
    {
        public static IWebHostBuilder ConfigureDefaultLoggging(this IWebHostBuilder builder)
        {
            return builder
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    context.HostingEnvironment.ConfigureNLog("nlog.config");
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
        }
    }
}
