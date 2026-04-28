using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Domain.Entities
{
    public class TaskEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        public TaskEntityStatus Status { get; set; } = TaskEntityStatus.ToDo;
        public TaskPriority Priority { get; set; } = TaskPriority.None;

        public DateTime? DueDate { get; set; }
        public List<CommentEntity> Comments { get; set; } = new();

        public Guid? ParentTaskId { get; set; }
        public TaskEntity? ParentTask { get; set; }
        public List<TaskEntity> Subtasks { get; set; } = new();

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid UserId { get; set; }

        public bool ReadyForClose => Subtasks.All(t => t.Status == TaskEntityStatus.Done);

        public int SortOrder { get; set; }
    }
}
