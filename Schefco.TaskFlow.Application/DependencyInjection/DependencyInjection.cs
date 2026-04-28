using Microsoft.Extensions.DependencyInjection;

namespace Schefco.TaskFlow.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        // This class is for adding any Application-level services
        // (Validators, mappers, domain service, etc.)
        // This is minimal because we created our own Mediator
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services;
        }
    }
}
