
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.AddComment
{
    public record AddCommentToTaskCommand(
        Guid TaskId,
        Guid UserId,
        string Content
    ) : ICommand<CommentDto>;
}
