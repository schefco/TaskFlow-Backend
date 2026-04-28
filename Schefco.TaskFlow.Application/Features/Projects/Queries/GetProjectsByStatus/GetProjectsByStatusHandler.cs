using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;
using Schefco.TaskFlow.Domain.Enums;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectsByStatus
{
    public class GetProjectsByStatusHandler : IQueryHandler<GetProjectsByStatusQuery, List<ProjectDTO>>
    {
        private readonly IProjectRepository _repository;

        public GetProjectsByStatusHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProjectDTO>> Handle(GetProjectsByStatusQuery request, CancellationToken cancellationToken)
        {
            List<Project> projects;

            // Check user role
            // If Owner, see everything
            // If not, see only those created by user
            if (request.Role == "Owner")
            {
                projects = await _repository.GetByStatusAsync(request.Status);
            }
            else
            {
                projects = await _repository.GetByStatusAndUserAsync(request.Status, request.UserId);
            }

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
