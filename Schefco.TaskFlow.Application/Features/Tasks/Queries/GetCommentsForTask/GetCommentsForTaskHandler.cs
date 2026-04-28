
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask
{
    public class GetCommentsForTaskHandler : IQueryHandler<GetCommentsForTaskQuery, List<CommentDto>>
    {
        private readonly ITaskRepository _taskRepo;

        public GetCommentsForTaskHandler(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public async Task<List<CommentDto>> Handle(GetCommentsForTaskQuery command, CancellationToken cancellationToken)
        {
            // fetch the comments for the task
            var comments = await _taskRepo.GetCommentsForTaskAsync(command.TaskId);

            // Return comment Dto
            return comments.Select(c => new CommentDto
            {
                Id = c.Id,
                TaskId = c.TaskId,
                UserId = c.UserId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }
    }
}
