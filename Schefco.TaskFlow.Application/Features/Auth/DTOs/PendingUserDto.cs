namespace Schefco.TaskFlow.Application.Features.Auth.DTOs
{
    public record PendingUserDto(
        Guid Id,
        string? Name,
        string Email,
        string? Company,
        string? Reason
        );
}
