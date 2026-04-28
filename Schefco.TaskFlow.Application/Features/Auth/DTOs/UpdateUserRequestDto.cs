namespace Schefco.TaskFlow.Application.Features.Auth.DTOs
{
    public record UpdateUserRequestDto(
        string Name,
        string? Company
    );
}
