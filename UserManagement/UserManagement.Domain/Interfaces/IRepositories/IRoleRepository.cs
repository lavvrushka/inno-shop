using UserManagement.Domain.Models;

namespace UserManagement.Domain.Interfaces.IRepositories
{
    public interface IRoleRepository : IRepository<Role>
    {

        Task<Role?> GetByNameAsync(string name);
    }
}
