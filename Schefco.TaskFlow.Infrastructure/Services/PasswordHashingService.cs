using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Schefco.TaskFlow.Infrastructure.Services
{
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly IPasswordHasher<AppUser> _inner;

        public PasswordHashingService(IPasswordHasher<AppUser> inner)
        {
            _inner = inner;
        }

        // Creates the password hash
        public string HashPassword(AppUser user, string password)
        {
            return _inner.HashPassword(user, password);
        }

        // Checks whether the provided password matches the stored hash
        // Uses ASP.NET Identity's verification logic to compare
        public bool VerifyPassword(AppUser user, string password)
        {
            var result = _inner.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
