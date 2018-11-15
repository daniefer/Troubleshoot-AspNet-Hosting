using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NewWorld.Integrations.WebApi.Configuration
{
    public static class DefaultWebHostBuilderExtensions
    {
        /// <summary>
        /// When we can swap this whole class out for WebHost.CreateDefaultBuilder<Startup>()
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webHostBuilder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseDefaultWebHostConfiguration<T>(this IWebHostBuilder webHostBuilder, string[] args) where T : class
        {
            return webHostBuilder
                .UseStartup<T>()
                .UseKestrel(ConfigureKestrel)
                .ConfigureAppConfiguration((ctx, config) => ConfigureApp(args, ctx, config))
                .ConfigureLogging(SetupLogging)
                .ConfigureLogging(SetupLogging)
                .ConfigureServices(ConfigureServices)
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });
        }

        /// <summary>
        /// When we can swap this whole class out for WebHost.CreateDefaultBuilder<Startup>()
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultWebHostFiltering(this IApplicationBuilder app)
        {
            return app.UseHostFiltering();
        }

        private static void ConfigureServices(WebHostBuilderContext hostingContext, IServiceCollection services)
        {
            // Fallback
            services.PostConfigure<HostFilteringOptions>(options =>
            {
                if (options.AllowedHosts == null || options.AllowedHosts.Count == 0)
                {
                    // "AllowedHosts": "localhost;127.0.0.1;[::1]"
                    var hosts = hostingContext.Configuration["AllowedHosts"]?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    // Fall back to "*" to disable.
                    options.AllowedHosts = (hosts?.Length > 0 ? hosts : new[] { "*" });
                }
            });
            // Change notification
            services.AddSingleton<IOptionsChangeTokenSource<HostFilteringOptions>>(
                new ConfigurationChangeTokenSource<HostFilteringOptions>(hostingContext.Configuration));
        }

        private static void ConfigureApp(string[] args, WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var env = hostingContext.HostingEnvironment;

            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null)
                {
                    config.AddUserSecrets(appAssembly, optional: true);
                }
            }

            config.AddEnvironmentVariables();

            if (args != null)
            {
                config.AddCommandLine(args);
            }
        }

        private static void ConfigureKestrel(WebHostBuilderContext builderContext, Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions options)
        {
            options.Configure(builderContext.Configuration.GetSection("Kestrel"));
        }

        private static void SetupLogging(WebHostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder
                .AddConfiguration(context.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug()
                .AddEventSourceLogger();
        }
    }
}