using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Projects.Commands.CreateProject
{
    public class CreateProjectHandler : ICommandHandler<CreateProjectCommand, CreateProjectResult>
    {
        private readonly IProjectRepository _repository;

        public CreateProjectHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateProjectResult> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            // Create the project
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                DueDate = request.DueDate,
                Status = request.Status,
                Priority = request.Priority,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByUserId = request.CreatedByUserId
            };

            await _repository.AddAsync(project);

            return new CreateProjectResult(
                project.Id,
                project.Name,
                project.Description,
                project.DueDate
            );
        }
    }
}
