using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;
using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectsByPriority
{
    public record GetProjectsByPriorityQuery(ProjectPriority Priority, Guid UserId, string Role) : IQuery<List<ProjectDTO>>;
}
