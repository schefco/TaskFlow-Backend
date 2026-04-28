using Microsoft.EntityFrameworkCore;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetByEmailAsync(string email)
        {
            // Gets user that exists in User Db
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(AppUser user)
        {
            // Drops the new user in User db from PendingUser db once reg is approved
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            // Check is email being used for registration already exists in User db
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<AppUser> GetByIdAsync(Guid id)
        {
            // Gets the user by their Id
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateAsync(AppUser user)
        {
            // Updates user data
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AppUser>> GetAllAsync()
        {
            // Gets all user from repo
            return await _context.Users.ToListAsync();
        }

        public async Task DeleteAsync(AppUser user)
        {
            // Delete user data
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
