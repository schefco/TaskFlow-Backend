using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjectById
{
    public class GetProjectByIdHandler : IQueryHandler<GetProjectByIdQuery, ProjectDTO>
    {
        private readonly IProjectRepository _repository;

        public GetProjectByIdHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProjectDTO> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            // load project from repository
            var project = await _repository.GetByIdAsync(request.Id);

            // If not found. Error
            if (project is null)
                throw new KeyNotFoundException($"Project with ID {request.Id} was not found");
            
            return new ProjectDTO(
                project.Id,
                project.Name,
                project.Description,
                project.DueDate,
                project.Status,
                project.Priority,
                project.CreatedAt,
                project.UpdatedAt
            );

            // throw new UnauthorizedAccessException("You cannot view another user's projects");
        }
    }
}
