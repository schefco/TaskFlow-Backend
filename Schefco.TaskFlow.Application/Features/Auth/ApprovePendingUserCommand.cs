using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public record ApprovePendingUserCommand(
        Guid pendingUserId
        ) : ICommand<string>;
}
