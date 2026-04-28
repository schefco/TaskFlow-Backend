using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Enums;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectsByPriority
{
    public class GetProjectsByPriorityHandler : IQueryHandler<GetProjectsByPriorityQuery, List<ProjectDTO>>
    {
        private readonly IProjectRepository _repository;

        public GetProjectsByPriorityHandler(IProjectRepository respository)
        {
            _repository = respository;
        }

        public async Task<List<ProjectDTO>> Handle(GetProjectsByPriorityQuery request, CancellationToken cancellationToken)
        {
            List<Project> projects;

            // Check user Role
            // If Owner, see everything
            // If not, see only those created by the user
            if (request.Role == "Owner")
            {
                projects = await _repository.GetByPriorityAsync(request.Priority);
            }
            else
            {
                projects = await _repository.GetByPriorityAndUserAsync(request.Priority, request.UserId);
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
