
using Microsoft.EntityFrameworkCore;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskEntity?> GetTaskByIdAsync(Guid id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskEntity> AddAsync(TaskEntity task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskEntity> UpdateAsync(TaskEntity task)
        {
            // add in updates, return updated entity
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is null)
                return;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Guid?> GetProjectIdForParentAsync(Guid parentTaskId)
        {
            // fetch the parent task without tracking since we only need the ProjectId
            var parent = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == parentTaskId);

            // return null if parent doesn't exist
            return parent?.ProjectId;
        }

        public async Task<List<TaskEntity>> GetSubtasksForTaskAsync(Guid taskId)
        {
            // fetch subtask for tasks that contain subtasks
            return await _context.Tasks.AsNoTracking().Where(t => t.ParentTaskId == taskId).ToListAsync();
        }

        public async Task<List<CommentEntity>> GetCommentsForTaskAsync(Guid taskId)
        {
            // Return sorted comments so the newest comment is always first
            return await _context.Comments.AsNoTracking()
                .Where(c => c.TaskId == taskId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<CommentEntity> AddCommentAsync(CommentEntity comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<CommentEntity?> GetCommentByIdAsync(Guid commentId)
        {
            // retuns the task comment by id
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task DeleteCommentAsync(CommentEntity comment)
        {
            // Delete the comment from the Comments DB
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<CommentEntity> UpdateCommentAsync(CommentEntity comment)
        {
            // add in updates, save changes, return updated comment
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> TaskHasSubtasksAsync(Guid taskId)
        {
            // Checks if a task has subtasks
            // Cannot delete a task that has subtasks
            // Subtasks need to be deleted before tasks
            return await _context.Tasks.AnyAsync(t => t.ParentTaskId == taskId);
        }
    }
}
