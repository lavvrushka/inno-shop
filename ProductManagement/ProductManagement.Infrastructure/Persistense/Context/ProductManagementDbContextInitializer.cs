using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace ProductManagement.Infrastructure.Persistense.Context
{
    public static class ProductManagementDbContextInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProductManagementDbContext>();

            await context.Database.MigrateAsync();

        }
    }
}
