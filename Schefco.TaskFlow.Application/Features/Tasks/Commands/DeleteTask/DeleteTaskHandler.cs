using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskHandler : ICommandHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IUserRepository _users;

        public DeleteTaskHandler(ITaskRepository taskRepo, IUserRepository users)
        {
            _taskRepo = taskRepo;
            _users = users;
        }

        public async Task<bool> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            // Load the task
            var task = await _taskRepo.GetTaskByIdAsync(command.TaskId);

            if (task == null)
                throw new Exception("Task not found");

            // Load the task creator
            var taskCreator = await _users.GetByIdAsync(task.UserId);
            if (taskCreator == null)
                throw new UnauthorizedAccessException("Task creator does not exist");

            // Load the current user
            var currentUser = await _users.GetByIdAsync(command.UserId);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found");

            // Authorization check
            // If the task was created by an Owner
            if (taskCreator.Role == "Owner")
            {
                // Only Owners can delete Owner-created tasks
                if (currentUser.Role != "Owner")
                    throw new UnauthorizedAccessException("Only Owners can delete Owner-created tasks");
            }
            else
            {
                // Task was created by a Recruiter
                // Recruiters can only delete their own tasks
                if (currentUser.Role != "Owner" && task.UserId != command.UserId)
                    throw new UnauthorizedAccessException("You do not have permission to delete this task");
            }

            // Check if the task has subtasks
            var hasSubtasks = await _taskRepo.TaskHasSubtasksAsync(command.TaskId);
            if (hasSubtasks)
                throw new Exception("Cannot delete a task that has subtasks.");

            // If all checks are good. Delete the task
            await _taskRepo.DeleteAsync(task.Id);

            // Return confirmation of delete
            return true;
        }
    }
}
