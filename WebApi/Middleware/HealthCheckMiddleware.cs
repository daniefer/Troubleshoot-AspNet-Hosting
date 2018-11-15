using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NewWorld.Integrations.WebApi.Middleware
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _healthCheckPath;

        public HealthCheckMiddleware(RequestDelegate next, string healthCheckPath)
        {
            _next = next;
            _healthCheckPath = healthCheckPath;
        }

        public Task InvokeAsync(HttpContext context, ILogger<HealthCheckMiddleware> logger)
        {
            if (context.Request.Path.Equals(_healthCheckPath))
            {
                logger.LogTrace("Health Check endpoint called. Configured to match {0}", _healthCheckPath);
                context.Response.ContentType = "application/json";
                var content = JsonConvert.SerializeObject(new HealthCheckResponse());
                return context.Response.WriteAsync(content);

            }
            return _next(context);
        }
    }
}
