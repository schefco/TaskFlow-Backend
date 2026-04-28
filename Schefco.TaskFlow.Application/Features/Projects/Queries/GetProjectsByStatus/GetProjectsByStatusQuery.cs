using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Domain.Enums;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectsByStatus
{
    public record GetProjectsByStatusQuery(ProjectStatus Status, Guid UserId, string Role) : IQuery<List<ProjectDTO>>;
}
