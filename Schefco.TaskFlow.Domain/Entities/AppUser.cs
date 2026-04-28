using System.ComponentModel.DataAnnotations;

namespace Schefco.TaskFlow.Domain.Entities
{
    //
    // User Class for RBAC
    //
    public class AppUser
    {
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Company { get; set; }
        public string PasswordHash { get; set; } = default!;
        public string Role { get; set; } = default!;
        public bool MustChangePassword { get; set; }

        public AppUser(Guid id, string name, string email, string? company, string passwordHash)
        {
            // store data from registration
            Id = id;
            Name = name;
            Email = email;
            Company = company;
            PasswordHash = passwordHash;

            // Each user defaults to Recruiter since they are not the owner
            Role = "Recruiter";

            // Each user that is transfered from PendingUsers will need to change password after first login
            MustChangePassword = true;
        }
    }
}
