
using Microsoft.EntityFrameworkCore;
using Schefco.TaskFlow.Application.Common.Interfaces;
using Schefco.TaskFlow.Domain.Entities;

namespace Schefco.TaskFlow.Infrastructure.Persistence.Repositories
{
    public class PendingUserRepository : IPendingUserRepository
    {
        private readonly AppDbContext _context;

        public PendingUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PendingUser user)
        {
            // Dropping the new pending user into the table so it can wait for approval
            _context.PendingUsers.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PendingUser>> GetAllAsync()
        {
            return await _context.PendingUsers.ToListAsync();
        }

        public async Task<PendingUser?> GetByIdAsync(Guid id)
        {
            // Grabbing the penidng user by their id - used when admin wants to approve/deny
            return await _context.PendingUsers.FindAsync(id);
        }

        public async Task DeleteAsync(PendingUser user)
        {
            // Once the user is approved and converted to a real AppUser, we clear that data from the PendingUser repo
            _context.PendingUsers.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistAsync(string email)
        {
            // Check is email being used for registration already exists in PendingUsers db
            return await _context.PendingUsers.AnyAsync(x => x.Email == email);
        }
    }
}
