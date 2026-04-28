using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectById
{
    public record GetProjectByIdQuery(Guid Id, Guid UserId, string Role) : IQuery<ProjectDTO>;
}
