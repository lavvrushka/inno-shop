using ProductManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Threading.Tasks;
using ProductManagement.Infrastructure.Persistense.Context;
using ProductManagement.Infrastructure.Persistense.Repositories;

namespace ProductManagement.Tests.RepositoryTests
{
    public class ProductRepositoryTest
    {
        private ProductManagementDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ProductManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ProductManagementDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProductToDatabase()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context);

            var product = new Product
            {
                Name = "Test Product",
                Description = "This is a test product.",
                Price = 99.99m,
                Quantity = 10,
                IsAvailable = true,
                ImageId = Guid.NewGuid()
            };

            // Act
            await repository.AddAsync(product);
            await context.SaveChangesAsync();

            // Assert
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");

            Assert.NotNull(savedProduct);
            Assert.Equal("Test Product", savedProduct.Name);
            Assert.Equal("This is a test product.", savedProduct.Description);
            Assert.Equal(99.99m, savedProduct.Price);
            Assert.Equal(10, savedProduct.Quantity);
            Assert.True(savedProduct.IsAvailable);
        }

        [Fact]
        public async Task GetProductCountAsync_ShouldReturnCorrectProductCount()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context);

            var product1 = new Product { Name = "Product 1", Description = "Desc 1", Price = 10.0m, Quantity = 5, IsAvailable = true, ImageId = Guid.NewGuid() };
            var product2 = new Product { Name = "Product 2", Description = "Desc 2", Price = 20.0m, Quantity = 3, IsAvailable = false, ImageId = Guid.NewGuid() };

            context.Products.AddRange(product1, product2);
            await context.SaveChangesAsync();

            // Act
            var productCount = await repository.GetProductCountAsync();

            // Assert
            Assert.Equal(2, productCount);
        }
    }
}