using Schefco.TaskFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Schefco.TaskFlow.Domain.Entities
{
    public class PendingUser
    {
        // Class for user pending approval from registration
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Company { get; set; } = string.Empty;
        public string? Reason { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public RegStatus Status { get; set; }

        public PendingUser(string name, string email, string? company, string reason)
        {
            // Generate the id right away so the frontend can use it
            Id = Guid.NewGuid();

            // Store the registration details as they came in
            Name = name;
            Email = email;
            Company = company;
            Reason = reason;

            // Timestamp for when the request was made
            RequestedAt = DateTime.UtcNow;

            // Every new request starts in Pending status
            Status = RegStatus.Pending;
        }
    }
}
