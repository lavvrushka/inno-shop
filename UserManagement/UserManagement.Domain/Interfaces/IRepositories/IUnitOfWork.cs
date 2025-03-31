namespace UserManagement.Domain.Interfaces.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {

        IRoleRepository Roles { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        Task<int> SaveChangesAsync();
    }
}
