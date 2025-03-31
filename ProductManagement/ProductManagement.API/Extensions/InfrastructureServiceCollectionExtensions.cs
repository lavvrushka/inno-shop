using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Interfaces.IRepositories;
using ProductManagement.Domain.Interfaces.IServices;
using ProductManagement.Infrastructure.Persistense;
using ProductManagement.Infrastructure.Persistense.Context;
using ProductManagement.Infrastructure.Persistense.Repositories;
using ProductManagement.Infrastructure.Services;

namespace ProductManagement.API.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            return services;
        }
    }
}
