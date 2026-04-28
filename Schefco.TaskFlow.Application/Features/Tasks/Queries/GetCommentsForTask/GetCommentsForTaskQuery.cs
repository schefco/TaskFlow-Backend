using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Queries.GetCommentsForTask
{
    public record GetCommentsForTaskQuery(Guid TaskId) : IQuery<List<CommentDto>>;
}
