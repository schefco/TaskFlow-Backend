using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Application.Common.Interfaces
{
    public interface IPasswordHashingService
    {
        // Abstraction for password hashing so Application doesn't depend on ASP.NET Identity
        string HashPassword(AppUser user, string password);
        bool VerifyPassword(AppUser user, string password);
    }
}
