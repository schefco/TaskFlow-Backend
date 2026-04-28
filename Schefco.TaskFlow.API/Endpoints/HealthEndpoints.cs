using Schefco.TaskFlow.API.Services;

namespace Schefco.TaskFlow.API.Endpoints
{
    public static class HealthEndpoints
    {
        public static void MapHealthEndpoints(this WebApplication app)
        {
            app.MapGet("health", (ITimeService timeService) =>
            {
                return new
                {
                    status = "OK",
                    service = "Schefco.TaskFlow",
                    timestamp = timeService.UtcNow(),
                };
            });
        }
    }
}
