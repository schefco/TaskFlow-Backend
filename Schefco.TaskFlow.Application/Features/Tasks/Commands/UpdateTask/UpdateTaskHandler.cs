using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.UpdateTask
{
    public class UpdateTaskHandler : ICommandHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepo;
        private readonly IUserRepository _users;

        public UpdateTaskHandler(ITaskRepository taskRepo, IUserRepository users)
        {
            _taskRepo = taskRepo;
            _users = users;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            // Load the current user
            var currentUser = await _users.GetByIdAsync(command.UserId);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found");

            // Check if task exist
            var task = await _taskRepo.GetTaskByIdAsync(command.TaskId);

            if (task == null)
                throw new Exception("Task not found");

            // Cannot update owner tasks or projects
            // Load the task creator
            var taskCreator = await _users.GetByIdAsync(task.UserId);
            if (taskCreator == null)
                throw new UnauthorizedAccessException("User not found");

            // If the task was created by an Owner
            if (taskCreator.Role == "Owner")
            {
                // Only owners can update Owner-created tasks
                if (currentUser.Role != "Owner")
                    throw new UnauthorizedAccessException("Only Owners can update Owner-created tasks");
            }
            else
            {
                // Task was created by a Recruiter
                // Recruiters can only update their own tasks
                if (currentUser.Role != "Owner" && task.UserId != command.UserId)
                    throw new UnauthorizedAccessException("You do not have permission to update this task");
            }

            // ParentTaskId safety check
            if (command.ParentTaskId == null)
            {
                // It becomes a top level task if is has no ParentTaskId
                task.ParentTaskId = null;
            }
            else
            {
                // Validate the parent task exists
                var parent = await _taskRepo.GetTaskByIdAsync(command.ParentTaskId.Value);
                if (parent == null)
                    throw new Exception("Parent task not found");

                // Parent nesting. In case we try to make sub task that has the same parent task id
                if (parent.Id == task.Id)
                    throw new Exception("A task cannot be its own parent");

                task.ParentTaskId = command.ParentTaskId;
            }

            // Apply updates
            task.Title = command.Title;
            task.Description = command.Description;
            task.Status = command.Status;
            task.DueDate = command.DueDate;
            task.Priority = command.Priority;
            task.ParentTaskId = command.ParentTaskId;
            task.UpdatedAt = DateTime.UtcNow;

            // Save changes
            var updated = await _taskRepo.UpdateAsync(task);

            // Return DTO
            return new TaskDto 
            {
                Id = updated.Id,
                ProjectId = updated.ProjectId,
                UserId = updated.UserId,
                Title = updated.Title,
                Description = updated.Description,
                Status = updated.Status,
                DueDate = updated.DueDate,
                Priority = updated.Priority,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            };
        }
    }
}
