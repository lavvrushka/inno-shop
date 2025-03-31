using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Infrastructure.Persistence.Context;
using UserManagement.Infrastructure.Persistence.Repositories;

namespace UserManagement.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManagementDbContext _context;
        private IUserRepository? _userRepository;
        private IRoleRepository? _roleRepository;
        private IRefreshTokenRepository? _refreshTokenRepository;
        public UnitOfWork(UserManagementDbContext context)
        {
            _context = context;
        }
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
