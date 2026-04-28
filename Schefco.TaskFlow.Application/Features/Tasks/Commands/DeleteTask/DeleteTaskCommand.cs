using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.DeleteTask
{
    // Cannot delete a task if it has subtasks
    public record DeleteTaskCommand(
        Guid TaskId,
        Guid UserId
    ) : ICommand<bool>;
}
