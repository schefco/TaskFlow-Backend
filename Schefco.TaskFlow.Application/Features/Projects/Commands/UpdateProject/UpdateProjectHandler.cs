using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Features.Projects.Queries.GetProjects;

namespace Schefco.TaskFlow.Application.Features.Projects.Commands.UpdateProject
{
    public class UpdateProjectHandler : ICommandHandler<UpdateProjectCommand, ProjectDTO>
    {
        private readonly IProjectRepository _repository;

        public UpdateProjectHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProjectDTO> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            // Get project from repository
            var project = await _repository.GetByIdAsync(request.Id);

            // If project not found
            if (project == null)
                throw new KeyNotFoundException($"Project with ID {request.Id} not found");

            // Owner can update anything
            if (request.Role == "Owner" || project.CreatedByUserId == request.UserId)
            {
                project.Name = request.Name;
                project.Description = request.Description;
                project.DueDate = request.DueDate;
                project.Status = request.Status;
                project.Priority = request.Priority;
                project.UpdatedAt = DateTime.UtcNow;

                await _repository.UpdateAsync(project);

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
            }

            throw new Exception("You cannot update another user's project.");
        }
    }
}
