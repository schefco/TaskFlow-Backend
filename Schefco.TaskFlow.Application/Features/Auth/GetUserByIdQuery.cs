using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public record GetUserByIdQuery(Guid UserId) : IQuery<UserDetailsDto>;
}
