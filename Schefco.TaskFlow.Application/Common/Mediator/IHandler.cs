using System;
using System.Collections.Generic;
using System.Text;

namespace Schefco.TaskFlow.Application.Common.Mediator
{
    // This is the contract for any command hangler in the app.
    // Bassically: Takes a command and returns a response
    public interface ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        // This is the method every command handler must implement
        // It is where the actual work happens (login, regiter, etc)
        Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
    }

    // Same basic operation as above but for queries
    // Command = "do something"
    // Queries = "get something"
    public interface IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        // Query handlers return data but don't change anything
        Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
    }
}
