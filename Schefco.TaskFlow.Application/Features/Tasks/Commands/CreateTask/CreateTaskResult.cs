using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskResult(
        Guid Id,
        string Title,
        string? Description,
        TaskEntityStatus Status,
        TaskPriority Priority,
        DateTime? DueDate,
        Guid ProjectId,
        Guid? ParentTaskId,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        DateTime? CompletedAt,
        Guid UserId,
        int SortOrder,
        List<CreateTaskResult> SubTasks
    );
}
