using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById;

namespace Schefco.TaskFlow.Application.Features.Tasks.Queries.GetSubtasks
{
    public record GetSubtasksForTaskQuery(Guid TaskId) : IQuery<List<TaskDto>>;
}
