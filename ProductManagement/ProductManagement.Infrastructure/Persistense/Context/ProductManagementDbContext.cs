using Microsoft.EntityFrameworkCore;
using ProductManagement.Domain.Models;
using ProductManagement.Infrastructure.Persistense.Comparers;
using ProductManagement.Infrastructure.Persistense.Configurations;

namespace ProductManagement.Infrastructure.Persistense.Context
{
    public class ProductManagementDbContext : DbContext
    {
        public ProductManagementDbContext(DbContextOptions<ProductManagementDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new ProductConfiguration());

            modelBuilder.Entity<Product>()
               .HasOne(e => e.Image)
               .WithMany()
               .HasForeignKey(e => e.ImageId)
               .OnDelete(DeleteBehavior.SetNull);
            var dictionaryComparer = new DictionaryValueComparer();
        }

    }
}
