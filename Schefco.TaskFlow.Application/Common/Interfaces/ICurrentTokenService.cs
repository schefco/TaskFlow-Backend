
namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    public interface ICurrentTokenService
    {
        string? Token { get; set; }
        Guid GetCurrentUserId();
    }
}
