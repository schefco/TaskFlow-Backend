using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;
using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskHandler : ICommandHandler<CreateTaskCommand, CreateTaskResult>
    {
        private readonly ITaskRepository _taskRepo;
        private readonly ICurrentTokenService _tokenService;

        public CreateTaskHandler(ITaskRepository taskRepo, ICurrentTokenService tokenService)
        {
            _taskRepo = taskRepo;
            _tokenService = tokenService;
        }

        public async Task<CreateTaskResult> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            Guid projectId;

            if (command.ParentTaskId is null)
            {
                // parent task to use the projectId from the request
                projectId = command.ProjectId;
            }
            else
            {
                // subtask to inheirt projectId from parent
                var parentProjectId = await _taskRepo.GetProjectIdForParentAsync(command.ParentTaskId.Value);

                if (parentProjectId is null)
                    throw new Exception("Parent task not found.");

                projectId = parentProjectId.Value;
            }

            // Get the current user ID from the JWT
            var userId = _tokenService.GetCurrentUserId();

            // build the new task entity from the incoming request
            var task = new TaskEntity
            {
                Id = Guid.NewGuid(), // generate new task id
                Title = command.Title,
                Description = command.Description,
                Status = command.Status,
                Priority = command.Priority,
                DueDate = command.DueDate,
                ParentTaskId = command.ParentTaskId,
                ProjectId = projectId,

                CreatedAt = DateTime.UtcNow, // timestamp for creation
                UpdatedAt = DateTime.UtcNow, // timestamp for update, same as creation on initial create

                UserId = userId,
                Comments = new List<CommentEntity>() // initialize an empty comments list
            };

            // Add the new task to the Task Repo
            await _taskRepo.AddAsync(task);

            return new CreateTaskResult(
                task.Id,
                task.Title,
                task.Description,
                task.Status,
                task.Priority,
                task.DueDate,
                task.ProjectId,
                task.ParentTaskId,
                task.CreatedAt,
                task.UpdatedAt,
                task.CompletedAt,
                task.UserId,
                task.SortOrder,
                new List<CreateTaskResult>()
            );
        }
    }
}
