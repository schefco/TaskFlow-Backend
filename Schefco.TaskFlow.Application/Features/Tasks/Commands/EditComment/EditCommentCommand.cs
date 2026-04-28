using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.EditComment
{
    public record EditCommentCommand(
        Guid CommentId,
        Guid UserId,
        string Content
    ) : ICommand<CommentDto>;
}
