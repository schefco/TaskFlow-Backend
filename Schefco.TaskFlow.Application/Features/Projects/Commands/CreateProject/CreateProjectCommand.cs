using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Application.Features.Projects.Commands.CreateProject
{
    public record CreateProjectCommand(
        string Name,
        string Description,
        DateTime? DueDate,
        ProjectStatus Status,
        ProjectPriority Priority,
        Guid? CreatedByUserId
        ) : ICommand<CreateProjectResult>;
}
