using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    public interface IPendingUserRepository
    {
        Task AddAsync(PendingUser user);
        Task<IEnumerable<PendingUser>> GetAllAsync();
        Task<PendingUser?> GetByIdAsync(Guid id);
        Task DeleteAsync(PendingUser user);
        Task<bool> EmailExistAsync(string email);
    }
}
