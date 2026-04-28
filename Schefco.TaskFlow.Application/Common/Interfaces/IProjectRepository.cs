using Schefco.TaskFlow.Domain.Entities;
using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> AddAsync(Project project);
        Task<Project?> GetByIdAsync(Guid id);
        Task<List<Project>> GetAllAsync();
        Task UpdateAsync(Project project);
        Task DeleteAsync(Guid id);
        Task<List<Project>> GetByStatusAsync(ProjectStatus status);
        Task<List<Project>> GetByStatusAndUserAsync(ProjectStatus status, Guid userId);
        Task<List<Project>> GetByPriorityAsync(ProjectPriority priority);
        Task<List<Project>> GetByPriorityAndUserAsync(ProjectPriority priority, Guid userId);
        Task<List<Project>> GetByUserIdAsync(Guid userId);
    }
}
