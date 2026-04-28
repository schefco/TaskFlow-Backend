using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByEmailAsync(string email);
        Task AddAsync(AppUser user);
        Task<bool> EmailExistsAsync(string email);
        Task<AppUser?> GetByIdAsync(Guid id);
        Task UpdateAsync(AppUser user);
        Task<List<AppUser>> GetAllAsync();
        Task DeleteAsync(AppUser user);
    }
}
