namespace Schefco.TaskFlow.Application.Features.Auth.DTOs
{
    public record UserDto(
        Guid Id,
        string Name,
        string Email,
        string? Company
    );
}
