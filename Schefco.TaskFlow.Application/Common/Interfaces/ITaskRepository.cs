using Schefco.TaskFlow.Application.Features.Tasks.Queries.GetTaskById;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetTaskByIdAsync(Guid id);
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity> AddAsync(TaskEntity task);
        Task<TaskEntity> UpdateAsync(TaskEntity task);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<Guid?> GetProjectIdForParentAsync(Guid parentTaskId);
        Task<List<TaskEntity>> GetSubtasksForTaskAsync(Guid taskId);
        Task<List<CommentEntity>> GetCommentsForTaskAsync(Guid taskId);
        Task<CommentEntity> AddCommentAsync(CommentEntity comment);
        Task<CommentEntity?> GetCommentByIdAsync(Guid commentId);
        Task DeleteCommentAsync(CommentEntity comment);
        Task<CommentEntity> UpdateCommentAsync(CommentEntity comment);
        Task<bool> TaskHasSubtasksAsync(Guid taskId);
    }
}
