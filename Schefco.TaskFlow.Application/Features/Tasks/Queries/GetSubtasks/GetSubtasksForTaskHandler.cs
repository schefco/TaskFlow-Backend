
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById;

namespace Schefco.TaskFlow.Application.Features.Tasks.Queries.GetSubtasks
{
    public class GetSubtasksForTaskHandler : IQueryHandler<GetSubtasksForTaskQuery, List<TaskDto>>
    {
        private readonly ITaskRepository _taskRepo;

        public GetSubtasksForTaskHandler(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public async Task<List<TaskDto>> Handle(GetSubtasksForTaskQuery command, CancellationToken cancellationToken)
        {
            // Feth direct subtasks
            var subtasks = await _taskRepo.GetSubtasksForTaskAsync(command.TaskId);

            // map to DTO
            return subtasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                ParentTaskId = t.ParentTaskId,
                ProjectId = t.ProjectId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList();
        }
    }
}
