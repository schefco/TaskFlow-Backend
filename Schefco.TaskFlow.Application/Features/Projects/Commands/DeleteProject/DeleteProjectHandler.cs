using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Common.Interfaces;

namespace Schefco.TaskFlow.Application.Features.Projects.Commands.DeleteProject
{
    public class DeleteProjectHandler : ICommandHandler<DeleteProjectCommand, bool>
    {
        private readonly IProjectRepository _repository;

        public DeleteProjectHandler(IProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            // Load in the project from repository
            var project = await _repository.GetByIdAsync(request.Id);

            // Not Found
            if (project == null)
                throw new Exception("Project not found.");

            //Owner can delete anything
            if (request.Role == "Owner")
            {
                await _repository.DeleteAsync(request.Id);
                return true;
            }

            // Other users can delete only their own projects
            if (request.Role == "Recruiter")
            {
                // Check if the requestor is the user who created the project
                // If not, error, Cannot delete another user's project
                if (project.CreatedByUserId != request.UserId)
                    throw new Exception("You cannot delete another user's project");

                await _repository.DeleteAsync(request.Id);
                return true;
            }

            // If the role is at all Invalid
            throw new Exception("Invalid role.");
        }
    }
}
