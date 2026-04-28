using Microsoft.Extensions.DependencyInjection;

namespace Schefco.TaskFlow.Application.Common.Mediator
{
    // This is the main entry point the API will call.
    // It figures out which handler to use and runs it
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command);
        Task<TResponse> Send<TResponse>(IQuery<TResponse> query);
    }

    // This class uses DI to locate the correct handler at runtime.
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _provider;

        public Mediator(IServiceProvider provider)
        {
            _provider = provider;
        }

        // Handles command (things that change state)
        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command)
        {
            // Buidl the handler type based on the command's actual type
            // Example: ICommandHandler<LoginCommand, LoginResponse>
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));

            // Resolve the handler from DI
            dynamic handler = _provider.GetRequiredService(handlerType);

            // Execute the handler
            return handler.Handle((dynamic)command, CancellationToken.None);
        }

        // Handles queries (things that return data)
        public Task<TResponse> Send<TResponse>(IQuery<TResponse> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));

            dynamic handler = _provider.GetRequiredService(handlerType);

            return handler.Handle((dynamic)query, CancellationToken.None);
        }
    }
}
