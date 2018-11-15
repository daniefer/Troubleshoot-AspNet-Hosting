using Microsoft.AspNetCore.Builder;

namespace NewWorld.Integrations.WebApi.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, string path)
        {
            return app.UseMiddleware<HealthCheckMiddleware>(path);
        }
    }
}
