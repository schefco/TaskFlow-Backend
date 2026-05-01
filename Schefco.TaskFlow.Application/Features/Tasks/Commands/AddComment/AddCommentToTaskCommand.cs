
using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Features.Tasks.Commands.AddComment
{
    public record AddCommentToTaskCommand(
        Guid TaskId,
        Guid UserId,
        string User,
        string Content
    ) : ICommand<CommentDto>;
}
