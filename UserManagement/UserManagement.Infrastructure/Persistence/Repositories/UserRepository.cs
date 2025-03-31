using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Models;
using UserManagement.Infrastructure.Persistence.Context;

namespace UserManagement.Infrastructure.Persistence.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(UserManagementDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public new async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public new async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<bool> IsUserActiveAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user!.IsActive;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task SetUserStatusAsync(Guid userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            user!.IsActive = isActive;
            _context.Users.Update(user);
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public async Task<User?> GetByEmailConfirmationTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
        }

        public async Task ConfirmEmailAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            user!.EmailVerifiedAt = DateTime.UtcNow;
            _context.Users.Update(user);
        }

        public async Task ResetPasswordAsync(Guid userId, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            user!.Password = newPasswordHash;
            _context.Users.Update(user);
        }

        public async Task UpdateEmailConfirmationTokenAsync(Guid userId, string token)
        {
            var user = await _context.Users.FindAsync(userId);
            user!.EmailConfirmationToken = token;
        }

        public async Task<User?> GetByAccountRecoveryTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.AccountRecoveryToken == token);
        }
    }
}
