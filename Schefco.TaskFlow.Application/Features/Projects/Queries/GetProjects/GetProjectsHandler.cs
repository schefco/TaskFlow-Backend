using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Sorting;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects
{
    public class GetProjectsHandler : IQueryHandler<GetProjectQuery, List<ProjectDTO>>
    {
        private readonly IProjectRepository _repository;

        public GetProjectsHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProjectDTO>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            // RBAC Filtering
            List<Project> projects = request.Role == "Owner"
                ? await _repository.GetAllAsync()
                : await _repository.GetByUserIdAsync(request.UserId);

            // Sorting
            projects = ProjectSorting.SortProjects(projects, request.SortBy, request.SortDirection);

            // Pagination
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 20 : request.PageSize;

            var skip = (pageNumber - 1) * pageSize;

            projects = projects.Skip(skip).Take(pageSize).ToList();

            // DTO Mapping
            return projects.Select(p => new ProjectDTO(
                p.Id,
                p.Name,
                p.Description,
                p.DueDate,
                p.Status,
                p.Priority,
                p.CreatedAt,
                p.UpdatedAt
            )).ToList();
        }
    }
}
