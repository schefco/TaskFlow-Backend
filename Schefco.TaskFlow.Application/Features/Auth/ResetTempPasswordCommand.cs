using Schefco.TaskFlow.Application.Features.Auth.DTOs;
using Schefco.TaskFlow.Application.Common.Mediator;

namespace Schefco.TaskFlow.Application.Features.Auth
{
    public class ResetTempPasswordCommand : ICommand<LoginResponseDto>
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
