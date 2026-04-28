using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public record UpdateUserCommand(
        Guid UserId,
        string Name,
        string? Company
    ) : ICommand<UpdateUserRequestDto>;
}
