using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Domain.Models;

namespace UserManagement.Infrastructure.Persistence.Context
{
    public static class UserManagementDbContextInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

            await context.Database.MigrateAsync();

            await SeedDataAsync(context);
        }

        private static async Task SeedDataAsync(UserManagementDbContext context)
        {
            if (!context.Set<Role>().Any())
            {
                context.Set<Role>().AddRange(
                    new Role { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin" },
                    new Role { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "User" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Set<User>().Any(u => u.FirstName == "admin"))
            {
                context.Set<User>().Add(new User
                {
                    FirstName = "admin",
                    LastName = "admin",
                    Password = "$2b$12$bnnk0YhYSzBaKn9p7nZDU.prIchEvm.UBOTnJNeHWnEBdLivbdiNm",
                    Email = "ivanlavrivwork@gmail.com",
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    EmailConfirmationToken = string.Empty
                });
                await context.SaveChangesAsync();
            }

        }
    }
}
