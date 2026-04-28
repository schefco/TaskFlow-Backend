using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects
{
    public record ProjectDTO(
        Guid Id,
        string Name,
        string Description,
        DateTime? DueDate,
        ProjectStatus Status,
        ProjectPriority Priority,
        DateTime CreatedAt,
        DateTime UpdatedAt
        );
}
