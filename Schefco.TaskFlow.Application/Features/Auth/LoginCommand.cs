using Schefco.TaskFlow.Application.Common.Mediator;
using Schefco.TaskFlow.Application.Features.Auth.DTOs;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    // Represents the login request coming from the client
    public class LoginCommand : ICommand<LoginResponseDto>
    {
        // The user's email attempting to log in
        public string Email { get; set; } = string.Empty;

        // The password the user is providing
        public string Password { get; set; } = string.Empty;
    }
}
