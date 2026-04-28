using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Projects.Commands.CreateProject
{
    public record CreateProjectResult(
        Guid Id,
        string Name,
        string Description,
        DateTime? DueDate
    );
}
