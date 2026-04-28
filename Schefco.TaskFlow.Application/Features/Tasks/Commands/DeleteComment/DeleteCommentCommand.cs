
using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.DeleteComment
{
    public record DeleteCommentCommand(
        Guid CommentId,
        Guid UserId
    ) : ICommand<bool>;
}
