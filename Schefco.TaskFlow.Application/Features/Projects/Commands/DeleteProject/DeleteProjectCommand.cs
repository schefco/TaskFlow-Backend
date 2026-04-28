using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Projects.Commands.DeleteProject
{
    public record DeleteProjectCommand(Guid Id, Guid UserId, string Role) : ICommand<bool>;
}
