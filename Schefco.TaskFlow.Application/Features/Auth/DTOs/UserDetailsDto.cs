namespace Schefco.TaskFlow.Application.Features.Auth.DTOs
{
    public record UserDetailsDto(
        Guid Id,
        string Name,
        string Email,
        string? Company,
        string Role
    );
}
