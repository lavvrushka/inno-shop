using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Models;
using UserManagement.Infrastructure.Persistence.Context;
using UserManagement.Infrastructure.Persistence.Repositories;

namespace UserManagement.Tests.RepositoriesTests
{
    public class UserRepositoryTests
    {
        private readonly UserManagementDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            // Настройка InMemory базы данных для тестов
            var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new UserManagementDbContext(options);
            _repository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                FirstName = "John", 
                LastName = "Doe",    
                Password = "password123",  
                Role = new Role { Name = "Admin" } 
            };

            await _context.Users.AddAsync(user);  
            await _context.SaveChangesAsync();    

            // Act
            var result = await _repository.GetByEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }


        [Fact]
        public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // Act
            var result = await _repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Email = "user1@example.com",
                FirstName = "John", 
                LastName = "Doe",    
                Password = "password123",  
                Role = new Role { Name = "User" }
            };
            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Email = "user2@example.com",
                FirstName = "Jane",  
                LastName = "Smith",  
                Password = "password123",  
                Role = new Role { Name = "User" } 
            };

            await _context.Users.AddAsync(user1);  
            await _context.Users.AddAsync(user2); 
            await _context.SaveChangesAsync();    

            // Act
            var result = await _repository.GetUserCountAsync();

            // Assert
            Assert.Equal(2, result);  
        }


        [Fact]
        public async Task IsUserActiveAsync_ShouldReturnTrue_WhenUserIsActive()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "activeuser@example.com",
                FirstName = "Active",  
                LastName = "User",     
                Password = "password123",  
                IsActive = true,      
                Role = new Role { Name = "User" } 
            };

            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsUserActiveAsync(user.Id);

            // Assert
            Assert.True(result);  
        }


        [Fact]
        public async Task IsUserActiveAsync_ShouldReturnFalse_WhenUserIsNotActive()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "inactiveuser@example.com",
                FirstName = "Inactive",  
                LastName = "User",       
                Password = "password123",  
                IsActive = false,        
                Role = new Role { Name = "User" } 
            };

           
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.IsUserActiveAsync(user.Id);

            // Assert
            Assert.False(result);  
        }

        [Fact]
        public async Task CheckEmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                FirstName = "John",  
                LastName = "Doe",   
                Password = "password123", 
                Role = new Role { Name = "Admin" } 
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.CheckEmailExistsAsync("test@example.com");

            // Assert
            Assert.True(result);
        }


        [Fact]
        public async Task CheckEmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
        {
            // Act
            var result = await _repository.CheckEmailExistsAsync("nonexistent@example.com");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserDetails()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                FirstName = "John",  
                LastName = "Doe",    
                Password = "password123", 
                IsActive = true,
                Role = new Role { Name = "User" }
            };

            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            
            user.FirstName = "UpdatedFirstName";
            user.LastName = "UpdatedLastName";

            // Act
            await _repository.UpdateUserAsync(user);
            var updatedUser = await _repository.GetByIdAsync(user.Id);

            // Assert
            Assert.Equal("UpdatedFirstName", updatedUser!.FirstName); 
            Assert.Equal("UpdatedLastName", updatedUser.LastName);     
        }


        [Fact]
        public async Task SetUserStatusAsync_ShouldChangeUserStatus()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@example.com",
                FirstName = "John", 
                LastName = "Doe",   
                Password = "password123",  
                IsActive = true,     
                Role = new Role { Name = "User" }  
            };

          
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _repository.SetUserStatusAsync(user.Id, false); 
            var updatedUser = await _repository.GetByIdAsync(user.Id);

            // Assert
            Assert.False(updatedUser!.IsActive);  
        }

    }
}
