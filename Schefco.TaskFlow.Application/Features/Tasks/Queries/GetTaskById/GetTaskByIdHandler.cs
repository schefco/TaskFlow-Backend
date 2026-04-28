using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdHandler : IQueryHandler<GetTaskByIdQuery, TaskDto>
    {
        private readonly ITaskRepository _taskRepo;

        public GetTaskByIdHandler(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public async Task<TaskDto> Handle(GetTaskByIdQuery command, CancellationToken cancellationToken)
        {
            // Fetch the task
            var task = await _taskRepo.GetTaskByIdAsync(command.TaskId);

            // throw if not found
            if (task is null)
                throw new Exception("Task not found.");

            // map to the DTO
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                ParentTaskId = task.ParentTaskId,
                ProjectId = task.ProjectId,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
