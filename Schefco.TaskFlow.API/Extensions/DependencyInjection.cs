using Schefco.TaskFlow.API.Services;
using Schefco.TaskFlow.Application.DependencyInjection;

namespace Schefco.TaskFlow.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddScoped<ITimeService, TimeService>();
            services.AddApplication();
            return services;
        }
    }
}
