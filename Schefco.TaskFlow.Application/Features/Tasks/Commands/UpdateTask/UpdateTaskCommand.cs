using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand(
        Guid TaskId,
        Guid UserId,
        string Title,
        string Description,
        TaskEntityStatus Status,
        DateTime? DueDate,
        TaskPriority Priority,
        Guid? ParentTaskId
        ) : ICommand<TaskDto>;
}
