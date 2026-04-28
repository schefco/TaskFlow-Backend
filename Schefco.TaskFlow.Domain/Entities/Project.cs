using Schefco.TaskFlow.Domain.Enums;

namespace Schefco.TaskFlow.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime? DueDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;
        public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
    }
}
