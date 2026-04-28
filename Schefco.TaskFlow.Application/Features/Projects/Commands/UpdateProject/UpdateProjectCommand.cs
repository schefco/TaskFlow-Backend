using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;
using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Application.Features.Projects.Commands.UpdateProject
{
    public record UpdateProjectCommand(
        Guid Id,
        string Name,
        string Description,
        DateTime? DueDate,
        ProjectStatus Status,
        ProjectPriority Priority,
        Guid UserId,
        string Role
        ) : ICommand<ProjectDTO>;
}
