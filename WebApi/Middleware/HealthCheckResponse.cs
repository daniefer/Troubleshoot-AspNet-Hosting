using System;

namespace NewWorld.Integrations.WebApi.Middleware
{
    public class HealthCheckResponse
    {
        public HealthCheckResponse()
        {
            Status = "healthy";
            Timestamp = DateTime.Now.ToUniversalTime();
        }

        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
