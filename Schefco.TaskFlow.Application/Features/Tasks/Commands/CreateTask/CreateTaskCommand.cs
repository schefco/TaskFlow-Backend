using Schefco.TaskFlow.Domain.Enums;
using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(
        string Title,
        string? Description,
        TaskEntityStatus Status,
        TaskPriority Priority,
        DateTime? DueDate,
        Guid? ParentTaskId,
        Guid ProjectId
    ) : ICommand<CreateTaskResult>;
}
