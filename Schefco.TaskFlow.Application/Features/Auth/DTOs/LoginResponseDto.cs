namespace Schefco.TaskFlow.Application.Features.Auth.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public bool RequiresPasswordReset { get; set; }
    }
}
