using Microsoft.EntityFrameworkCore;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;
using Schefco.TaskFlow.Domain.Enums;
using Schefco.TaskFlow.Infrastructure.Persistence;

namespace Schefco.TaskFlow.Infrastructure.Persistence.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        // This is a layer to protect Application from interacting with our DB directly
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Project> AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> GetByIdAsync(Guid id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project is null)
                return;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Project>> GetByStatusAsync(ProjectStatus status)
        {
            return await _context.Projects.Where(p => p.Status == status).ToListAsync();
        }

        public async Task<List<Project>> GetByStatusAndUserAsync(ProjectStatus status, Guid userId)
        {
            return await _context.Projects.Where(p => p.Status == status && p.CreatedByUserId == userId).ToListAsync();
        }

        public async Task<List<Project>> GetByPriorityAsync(ProjectPriority priority)
        {
            return await _context.Projects.Where(p => p.Priority == priority).ToListAsync();
        }

        public async Task<List<Project>> GetByPriorityAndUserAsync(ProjectPriority priority, Guid userId)
        {
            return await _context.Projects.Where(p => p.Priority == priority && p.CreatedByUserId == userId).ToListAsync();
        }

        public async Task<List<Project>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Projects.Where(p => p.CreatedByUserId == userId).ToListAsync();
        }
    }
}
