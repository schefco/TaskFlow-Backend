
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.AddComment
{
    public class AddCommentToTaskHandler : ICommandHandler<AddCommentToTaskCommand, CommentDto>
    {
        private readonly ITaskRepository _taskRepo;

        public AddCommentToTaskHandler(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public async Task<CommentDto> Handle(AddCommentToTaskCommand command, CancellationToken cancellationToken)
        {
            // Ensure the task exists
            var task = await _taskRepo.GetTaskByIdAsync(command.TaskId);

            if (task is null)
                throw new Exception("Task not found.");

            // Create the new comment entity
            var comment = new CommentEntity
            {
                Id = Guid.NewGuid(),
                TaskId = command.TaskId,
                UserId = command.UserId,
                User = command.User,
                Content = command.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save it to the DB
            var saved = await _taskRepo.AddCommentAsync(comment);

            // Return the DTO
            return new CommentDto
            {
                Id = saved.Id,
                TaskId = saved.TaskId,
                UserId = saved.UserId,
                Content = saved.Content,
                CreatedAt = saved.CreatedAt,
                UpdatedAt = saved.UpdatedAt
            };
        }
    }
}
