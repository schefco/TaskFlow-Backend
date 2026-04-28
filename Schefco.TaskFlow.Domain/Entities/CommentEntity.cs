
namespace Schefco.TaskFlow.Domain.Entities
{
    public class CommentEntity
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public TaskEntity Task { get; set; }
    }
}
