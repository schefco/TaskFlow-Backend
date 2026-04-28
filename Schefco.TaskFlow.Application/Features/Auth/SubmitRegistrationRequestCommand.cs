using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public record SubmitRegistrationRequestCommand(
        string Name,
        string Email,
        string? Company,
        string Reason
    ) : ICommand<string>;
}
