using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById
{
    public record GetTaskByIdQuery(Guid TaskId) : IQuery<TaskDto>;
}
