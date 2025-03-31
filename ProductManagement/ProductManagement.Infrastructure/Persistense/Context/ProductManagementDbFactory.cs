using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProductManagement.Infrastructure.Persistense.Context;
using System.IO;

namespace ProductManagement.Infrastructure.Persistence.Context
{
    public class ProductManagementDbFactory : IDesignTimeDbContextFactory<ProductManagementDbContext>
    {
        public ProductManagementDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("C:\\Users\\bebrik\\Desktop\\innshop-app\\ProductManagement\\ProductManagement.API\\appsettings.json") 
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ProductManagementDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ProductManagementDbContext(optionsBuilder.Options);
        }
    }
}
