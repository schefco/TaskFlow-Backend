using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects
{
    public record GetProjectQuery(
        Guid UserId,
        string Role,
        string? SortBy,
        string? SortDirection,
        int PageNumber,
        int PageSize
    ) : IQuery<List<ProjectDTO>>;
}
